using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using Remutable.Extensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using SubscriptionsDb;
using TdLib;
using Telegram.Bot.Types.InlineQueryResults;
using TelegramClient;
using Message = Common.Message;
using Update = Common.Update;
using User = Common.User;

namespace TelegramSender
{
    public class MessagesConsumer : IConsumer<Message>
    {
        private readonly IChatSubscriptionsRepository _repository;
        private readonly IProducer<ChatSubscriptionRequest> _producer;
        private readonly ISenderFactory _senderFactory;
        private readonly MessageBuilder _messageBuilder;
        private readonly ILogger<MessagesConsumer> _logger;
        private readonly ConcurrentDictionary<ChatId, ActionBlock<Task>> _chatSenders;
        private MessageSender _sender;
        private static readonly TimeSpan InvalidSubscriptionExpiration = TimeSpan.FromDays(1);

        public MessagesConsumer(
            IChatSubscriptionsRepository repository,
            IProducer<ChatSubscriptionRequest> producer,
            ISenderFactory senderFactory,
            MessageBuilder messageBuilder,
            ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _producer = producer;
            _senderFactory = senderFactory;
            _messageBuilder = messageBuilder;
            _logger = loggerFactory.CreateLogger<MessagesConsumer>();
            _chatSenders = new ConcurrentDictionary<ChatId, ActionBlock<Task>>();
        }

        public async Task ConsumeAsync(Message message, CancellationToken token)
        {
            _sender ??= await _senderFactory.CreateAsync();
            
            _logger.LogInformation("Received {}", message);
            
            var uploadedContents = (await SendFirstChatMessage(message)).ToList();

            foreach (UserChatSubscription chatInfo in message.DestinationChats.Skip(1))
            {
                ParsedMessageInfo parsedMessageInfo = await GetParsedMessageInfo(chatInfo, message.Update);
                
                IEnumerable<TdApi.InputMessageContent> messageContents = WithUploadedContents(parsedMessageInfo.Media, uploadedContents);

                await SendChatMessage(message, chatInfo, parsedMessageInfo with { Media = messageContents });
            }
        }

        private async Task<IEnumerable<TdApi.InputMessageContent>> SendFirstChatMessage(Message message)
        {
            IEnumerable<TdApi.Message> sentMessages = await SendSingleChatMessage(message, message.DestinationChats.First());
            
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
                .Where(m => !(m.Content is TdApi.MessageContent.MessageText))
                .Select(m => m.Content.ToInputMessageContent());
        }

        private async Task<IEnumerable<TdApi.Message>> SendSingleChatMessage(Message message, UserChatSubscription chatInfo)
        {
            string chatId = chatInfo.ChatId;
            Update update = message.Update;

            ParsedMessageInfo parsed = await GetParsedMessageInfo(chatInfo, update);

            return await SendAsync(_sender, parsed, update, chatId);
        }

        private async Task<ParsedMessageInfo> GetParsedMessageInfo(UserChatSubscription chatInfo, Update update)
        {
            MessageInfo messageInfo = _messageBuilder.Build(update, chatInfo);
            
            return await _sender.ParseAsync(messageInfo);
        }

        private async Task SendChatMessage(Message message, UserChatSubscription chatInfo, ParsedMessageInfo messageInfo)
        {
            _logger.LogInformation("Sending update {} to chat id {}", message.Update, ((ChatId) chatInfo.ChatId).Username ?? ((ChatId) chatInfo.ChatId).Identifier.ToString());
            
            ActionBlock<Task> chatSender = _chatSenders
                .GetOrAdd(chatInfo.ChatId, _ => new ActionBlock<Task>(task => task));

            await chatSender.SendAsync(
                SendAsync(_sender, messageInfo, message.Update, chatInfo.ChatId));
                
            _logger.LogInformation("Successfully sent update {} to chat id {}", message.Update, ((ChatId) chatInfo.ChatId).Username ?? ((ChatId) chatInfo.ChatId).Identifier.ToString());
        }

        private async Task<IEnumerable<TdApi.Message>> SendAsync(
            MessageSender sender,
            ParsedMessageInfo message,
            Update originalUpdate,
            ChatId chat)
        {
            try
            {
                return await sender.SendAsync(message);
            }
            catch (MessageSendFailedException e)
            {
                if (e.Message == "Forbidden: bot was blocked by the user" ||
                    e.Message == "Bad Request: need administrator rights in the channel chat")
                {
                    await RemoveChatSubscription(originalUpdate.Author, chat);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to send update {} to chat id {}", originalUpdate, chat.Username ?? chat.Identifier.ToString());
            }

            return Enumerable.Empty<TdApi.Message>();
        }

        private async Task RemoveChatSubscription(User author, string chatId)
        {
            if (await CanSubscriptionBeRemoved(author, chatId))
            {
                return;
            }

            _logger.LogInformation("Removing subscription of {} from chat {}", author, chatId);
            await _repository.RemoveAsync(author, chatId);

            if (! await _repository.ExistsAsync(author))
            {
                _producer.Send(
                    new ChatSubscriptionRequest(
                        SubscriptionType.Unsubscribe,
                        new Subscription(author, null), 
                        chatId));
            }
        }

        private async Task<bool> CanSubscriptionBeRemoved(User author, string chatId)
        {
            var subscription = await _repository.GetAsync(author);
            var userChatSubscription = subscription.Chats.FirstOrDefault(chatSubscription => chatSubscription.ChatId == chatId);

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
