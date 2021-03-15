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
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using SubscriptionsDb;
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
            
            foreach (UserChatSubscription chatInfo in message.DestinationChats)
            {
                string chatId = chatInfo.ChatId;

                (Update update, _) = message;
            
                MessageInfo messageInfo = _messageBuilder.Build(update, chatInfo);

                await SendChatUpdate(update, _sender, messageInfo, chatId);
            }
        }

        private async Task SendChatUpdate(
            Update originalUpdate,
            MessageSender sender,
            MessageInfo message,
            ChatId chatId)
        {
            _logger.LogInformation("Sending update {} to chat id {}", originalUpdate, chatId.Username ?? chatId.Identifier.ToString());
            
            ActionBlock<Task> chatSender = _chatSenders
                .GetOrAdd(chatId, _ => new ActionBlock<Task>(task => task));

            await chatSender.SendAsync(
                SendAsync(sender, message, originalUpdate, chatId));
                
            _logger.LogInformation("Successfully sent update {} to chat id {}", originalUpdate, chatId.Username ?? chatId.Identifier.ToString());
        }

        private async Task SendAsync(
            MessageSender sender,
            MessageInfo message,
            Update originalUpdate,
            ChatId chat)
        {
            try
            {
                await sender.SendAsync(message);
            }
            catch (ChatNotFoundException)
            {
                await RemoveChatSubscription(originalUpdate.Author, chat);
            }
            catch (ApiRequestException e)
            {
                if (e.Message == "Forbidden: bot was blocked by the user" ||
                    e.Message == "Bad Request: need administrator rights in the channel chat")
                {
                    await RemoveChatSubscription(originalUpdate.Author, chat);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to send update {} to chat id {}", originalUpdate, chat.Username ?? chat.Identifier.ToString());
            }
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
