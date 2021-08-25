using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.RabbitMq.Common;
using Telegram.Bot.Types;
using SubscriptionsDb;
using TdLib;
using TelegramClient;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Message = Common.Message;
using User = Common.User;

namespace TelegramSender
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly IChatSubscriptionsRepository _repository;
        private readonly ISenderFactory _senderFactory;
        private readonly MessageInfoBuilder _messageInfoBuilder;
        private readonly ILogger<MessageConsumer> _logger;
        private readonly ConcurrentDictionary<ChatId, ActionBlock<Task>> _chatSenders;
        private MessageSender _sender;
        
        private static readonly TimeSpan InvalidSubscriptionExpiration = TimeSpan.FromDays(1);
        private static readonly string[] RemoveSubscriptionOnMessages =
        {
            "Bot was blocked by the user",
            "Bad Request: need administrator rights in the channel chat",
            "Have no write access to the chat"
        };

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Converters = { new MediaJsonConverter() }
        };

        public MessageConsumer(
            IChatSubscriptionsRepository repository,
            ISenderFactory senderFactory,
            MessageInfoBuilder messageInfoBuilder,
            ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _senderFactory = senderFactory;
            _messageInfoBuilder = messageInfoBuilder;
            _logger = loggerFactory.CreateLogger<MessageConsumer>();
            _chatSenders = new ConcurrentDictionary<ChatId, ActionBlock<Task>>();
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            Message message = context.Message;
            
            _sender ??= await _senderFactory.CreateAsync();

            _logger.LogInformation("Received {}", message);
            
            await ConsumeMessageAsync(message);
        }

        private async Task ConsumeMessageAsync(Message message)
        {
            // The message is first sent to a specific chat, and its uploaded media is then used to be sent concurrently to the remaining chats.
            // This is implemented in order to make sure files are only uploaded once to Telegram's servers.
            List<TdApi.InputMessageContent> uploadedContents = (await SendFirstChatMessage(message)).ToList();

            foreach (UserChatSubscription chatInfo in message.DestinationChats.Skip(1))
            {
                ParsedMessageInfo parsedMessageInfo = await GetParsedMessageInfo(chatInfo, message.NewPost);

                if (parsedMessageInfo == null)
                {
                    continue;
                }

                IEnumerable<TdApi.InputMessageContent> originalContents = parsedMessageInfo.Media;
                
                IEnumerable<TdApi.InputMessageContent> messageContents = uploadedContents.Any() 
                    ? WithUploadedContents(originalContents, uploadedContents) 
                    : originalContents;

                await SendChatMessage(message, chatInfo, parsedMessageInfo with { Media = messageContents });
            }
        }

        private async Task<IEnumerable<TdApi.InputMessageContent>> SendFirstChatMessage(Message message)
        {
            UserChatSubscription chatSubscription = message.DestinationChats.First();
            
            IEnumerable<TdApi.Message> sentMessages = await SendSingleChatMessage(message, chatSubscription);
            
            _logger.LogInformation("Successfully sent update {} to chat id {}", message.NewPost, chatSubscription.ChatInfo.Id);
            
            return GetInputMessageContents(sentMessages);
        }

        private static IEnumerable<TdApi.InputMessageContent> WithUploadedContents(IEnumerable<TdApi.InputMessageContent> content, IEnumerable<TdApi.InputMessageContent> uploadedContent)
        {
            return content
                .Zip(
                    uploadedContent,
                    (original, uploaded) => original.HasCaption(out TdApi.FormattedText caption)
                        ? uploaded.WithCaption(caption)
                        : original);
        }

        private static IEnumerable<TdApi.InputMessageContent> GetInputMessageContents(IEnumerable<TdApi.Message> messages)
        {
            return messages
                .Where(m => m.Content is not TdApi.MessageContent.MessageText)
                .Select(m => m.Content.ToInputMessageContentAsync());
        }

        private async Task<IEnumerable<TdApi.Message>> SendSingleChatMessage(Message message, UserChatSubscription chatInfo)
        {
            var chatId = chatInfo.ChatInfo.Id;
            var newPost = message.NewPost;

            ParsedMessageInfo parsed = await GetParsedMessageInfo(chatInfo, newPost);

            if (parsed == null)
            {
                return Enumerable.Empty<TdApi.Message>();
            }

            return await SendAsync(_sender, parsed, newPost, chatId);
        }

        private async Task<ParsedMessageInfo> GetParsedMessageInfo(UserChatSubscription subscription, NewPost newPost)
        {
            try
            {
                MessageInfo messageInfo = _messageInfoBuilder.Build(newPost, subscription);
            
                return await _sender.ParseAsync(messageInfo);
            }
            catch (TdException e)
            {
                if (e.Message == "CHANNEL_INVALID")
                {
                    // TODO remove chat subscription
                    //await RemoveChatSubscription(newPost.Author, subscription.ChatInfo.Id);
                }
            }
            return null;
        }

        private async Task SendChatMessage(Message message, UserChatSubscription chatInfo, ParsedMessageInfo messageInfo)
        {
            _logger.LogInformation("Sending update {} to chat id {}", message.NewPost, chatInfo.ChatInfo.Id);
            
            ActionBlock<Task> chatSender = _chatSenders
                .GetOrAdd(chatInfo.ChatInfo.Id, _ => new ActionBlock<Task>(task => task));

            await chatSender.SendAsync(
                SendAsync(_sender, messageInfo, message.NewPost, chatInfo.ChatInfo.Id));
                
            _logger.LogInformation("Successfully sent update {} to chat id {}", message.NewPost, chatInfo.ChatInfo.Id);
        }

        private async Task<IEnumerable<TdApi.Message>> SendAsync(
            MessageSender sender,
            ParsedMessageInfo message,
            NewPost originalPost,
            long chat)
        {
            try
            {
                return await sender.SendAsync(message);
            }
            catch (MessageSendFailedException e)
            {
                foreach (var media in originalPost.Post.MediaItems)
                {
                    _logger.LogWarning(JsonSerializer.Serialize(media, _jsonSerializerOptions));
                }
                await HandleException(originalPost, chat, e);
            }
            catch (TdException e)
            {
                await HandleException(originalPost, chat, e);
            }
            catch (Exception e)
            {
                Report(originalPost, chat, e);
            }

            return Enumerable.Empty<TdApi.Message>();
        }

        private void Report(NewPost originalPost, long chat, Exception e)
        {
            _logger.LogError(e, "Failed to send update {} to chat id {}", originalPost, chat);
        }

        private async Task HandleException(NewPost originalPost, long chat, Exception e)
        {
            // TODO Remove chat subscription
            if (RemoveSubscriptionOnMessages.Contains(e.Message))
            {
                //await RemoveChatSubscription(originalPost.Author, chat);
            }
            else
            {
                Report(originalPost, chat, e);
                throw e;
            }
        }

        private async Task RemoveChatSubscription(User author, long chatId)
        {
            // if (await CanSubscriptionBeRemoved(author, chatId))
            // {
            //     return;
            // }
            //
            // _logger.LogInformation("Removing subscription of {} from chat {}", author, chatId);
            // await _repository.RemoveAsync(author, chatId);
            //
            // if (! await _repository.ExistsAsync(author))
            // {
            //     _producer.Send(
            //         new ChatSubscriptionRequest(
            //             SubscriptionType.Unsubscribe,
            //             new Subscription(author, null), 
            //             chatId));
            // }
        }

        private async Task<bool> CanSubscriptionBeRemoved(User author, long chatId)
        {
            var subscription = await _repository.GetAsync(author);
            var userChatSubscription = subscription.Chats.FirstOrDefault(chatSubscription => chatSubscription.ChatInfo.Id == chatId);

            DateTime now = DateTime.Now;
            DateTime? subscriptionDate = userChatSubscription?.SubscriptionDate;

            return now - subscriptionDate > InvalidSubscriptionExpiration;
        }

        public async Task FlushAsync()
        {
            Task[] completions = _chatSenders
                .Select(CompleteAsync)
                .ToArray();

            await Task.WhenAll(completions);
        }

        private Task CompleteAsync(KeyValuePair<ChatId, ActionBlock<Task>> pair)
        {
            (ChatId chatId, ActionBlock<Task> chatSender) = pair;

            _logger.LogInformation("Completing chat sender for chat id: {}", chatId);

            return chatSender.CompleteAsync();
        }
    }
}
