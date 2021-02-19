using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Message = Common.Message;
using Update = Common.Update;

namespace TelegramSender
{
    public class MessagesConsumer : IConsumer<Message>
    {
        private readonly ISenderFactory _senderFactory;
        private readonly MessageBuilder _messageBuilder;
        private readonly ILogger<MessagesConsumer> _logger;
        private readonly ConcurrentDictionary<ChatId, ActionBlock<Task>> _chatSenders;
        private MessageSender _sender;

        public MessagesConsumer(
            ISenderFactory senderFactory,
            MessageBuilder messageBuilder,
            ILoggerFactory loggerFactory)
        {
            _senderFactory = senderFactory;
            _messageBuilder = messageBuilder;
            _logger = loggerFactory.CreateLogger<MessagesConsumer>();
            _chatSenders = new ConcurrentDictionary<ChatId, ActionBlock<Task>>();
        }

        public async Task ConsumeAsync(Message message, CancellationToken token)
        {
            _sender ??= await _senderFactory.CreateAsync();
            
            _logger.LogInformation("Received {}", message);
            
            foreach (var chatInfo in message.DestinationChats)
            {
                MessageInfo messageInfo = _messageBuilder.Build(message.Update, chatInfo);

                await SendChatUpdate(message.Update, _sender, messageInfo, chatInfo.ChatId);
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
                sender.SendAsync(message));
                
            _logger.LogInformation("Successfully sent update {} to chat id {}", originalUpdate, chatId.Username ?? chatId.Identifier.ToString());
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
