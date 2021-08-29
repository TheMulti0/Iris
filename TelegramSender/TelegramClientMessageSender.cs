using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Common;
using Microsoft.Extensions.Logging;
using Scraper.RabbitMq.Common;
using TdLib;
using Telegram.Bot.Types;
using TelegramClient;

namespace TelegramSender
{
    public class TelegramClientMessageSender : ITelegramMessageSender
    {
        private readonly ISenderFactory _senderFactory;
        private readonly MessageInfoBuilder _messageInfoBuilder;
        private readonly ILogger<TelegramClientMessageSender> _logger;
        private readonly ConcurrentDictionary<ChatId, ActionBlock<Task>> _chatSenders;
        private MessageSender _sender;
        
        private static readonly string[] RemoveSubscriptionOnMessages =
        {
            "Bot was blocked by the user",
            "Bad Request: need administrator rights in the channel chat",
            "Have no write access to the chat"
        };

        public TelegramClientMessageSender(
            ISenderFactory senderFactory,
            MessageInfoBuilder messageInfoBuilder,
            ILoggerFactory loggerFactory)
        {
            _senderFactory = senderFactory;
            _messageInfoBuilder = messageInfoBuilder;
            _logger = loggerFactory.CreateLogger<TelegramClientMessageSender>();
            _chatSenders = new ConcurrentDictionary<ChatId, ActionBlock<Task>>();
        }

        public async Task ConsumeAsync(SendMessage message, CancellationToken ct)
        {
            _sender ??= await _senderFactory.CreateAsync();

            await ConsumeMessageAsync(message, ct);
        }

        private async Task ConsumeMessageAsync(SendMessage message, CancellationToken ct)
        {
            // The message is first sent to a specific chat, and its uploaded media is then used to be sent concurrently to the remaining chats.
            // This is implemented in order to make sure files are only uploaded once to Telegram's servers.
            List<TdApi.InputMessageContent> uploadedContents = (await SendFirstChatMessage(message, ct)).ToList();

            foreach (UserChatSubscription chatInfo in message.DestinationChats.Skip(1))
            {
                ParsedMessageInfo parsedMessageInfo = await GetParsedMessageInfo(chatInfo, message.NewPost, ct);

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

        private async Task<IEnumerable<TdApi.InputMessageContent>> SendFirstChatMessage(SendMessage message, CancellationToken ct)
        {
            UserChatSubscription chatSubscription = message.DestinationChats.First();
            
            IEnumerable<TdApi.Message> sentMessages = await SendSingleChatMessage(message, chatSubscription, ct);
            
            _logger.LogInformation("Successfully sent {} to chat id {}", message.NewPost.Post.Url, chatSubscription.ChatInfo.Id);
            
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
                .Select(m => m.Content.ToInputMessageContent());
        }

        private async Task<IEnumerable<TdApi.Message>> SendSingleChatMessage(SendMessage message, UserChatSubscription chatInfo, CancellationToken ct)
        {
            var chatId = chatInfo.ChatInfo.Id;
            var newPost = message.NewPost;

            ParsedMessageInfo parsed = await GetParsedMessageInfo(chatInfo, newPost, ct);

            if (parsed == null)
            {
                return Enumerable.Empty<TdApi.Message>();
            }

            return await SendAsync(_sender, parsed, newPost, chatId);
        }

        private async Task<ParsedMessageInfo> GetParsedMessageInfo(UserChatSubscription subscription, NewPost newPost, CancellationToken ct)
        {
            try
            {
                MessageInfo messageInfo = _messageInfoBuilder.Build(newPost, subscription, ct);
            
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

        private async Task SendChatMessage(SendMessage message, UserChatSubscription chatInfo, ParsedMessageInfo messageInfo)
        {
            _logger.LogInformation("Sending {} to chat id {}", message.NewPost.Post.Url, chatInfo.ChatInfo.Id);
            
            ActionBlock<Task> chatSender = _chatSenders
                .GetOrAdd(chatInfo.ChatInfo.Id, _ => new ActionBlock<Task>(task => task));

            await chatSender.SendAsync(
                SendAsync(_sender, messageInfo, message.NewPost, chatInfo.ChatInfo.Id));
                
            _logger.LogInformation("Successfully sent {} to chat id {}", message.NewPost.Post.Url, chatInfo.ChatInfo.Id);
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
                    _logger.LogWarning(media.Url);
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
            _logger.LogError(e, "Failed to send {} to chat id {}", originalPost.Post.Url, chat);
        }

        private async Task HandleException(NewPost originalPost, long chat, Exception e)
        {
            if (RemoveSubscriptionOnMessages.Contains(e.Message))
            {
                // TODO Remove chat subscription
            }
            else
            {
                Report(originalPost, chat, e);
                throw e;
            }
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