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
using UserDataLayer;
using Message = Common.Message;
using Update = Common.Update;
using User = Common.User;

namespace TelegramSender
{
    public class MessagesConsumer : IConsumer<Message>
    {
        private readonly ISavedUsersRepository _repository;
        private readonly IProducer<ChatSubscriptionRequest> _producer;
        private readonly ISenderFactory _senderFactory;
        private readonly MessageBuilder _messageBuilder;
        private readonly ILogger<MessagesConsumer> _logger;
        private readonly ConcurrentDictionary<ChatId, ActionBlock<Task>> _chatSenders;
        private MessageSender _sender;

        public MessagesConsumer(
            ISavedUsersRepository repository,
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
                SendAsync(sender, message, originalUpdate.Author, chatId));
                
            _logger.LogInformation("Successfully sent update {} to chat id {}", originalUpdate, chatId.Username ?? chatId.Identifier.ToString());
        }

        private async Task SendAsync(
            MessageSender sender,
            MessageInfo message,
            User author,
            ChatId chat)
        {
            try
            {
                await sender.SendAsync(message);
            }
            catch (ChatNotFoundException)
            {
                await RemoveChatSubscription(author, chat);
            }
            catch (ApiRequestException e)
            {
                if (e.Message == "Forbidden: bot was blocked by the user" ||
                    e.Message == "Bad Request: need administrator rights in the channel chat") // TODO dont unsubscribe if time between subscription to update is less than x
                {
                    await RemoveChatSubscription(author, chat);    
                }
            }
        }

        private async Task RemoveChatSubscription(User author, string chatId)
        {
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
