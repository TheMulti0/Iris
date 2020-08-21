using System;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Extensions;

namespace TelegramConsumer
{
    internal class ChatSender : IDisposable
    {
        private readonly MessageSender _messageSender;
        private readonly Channel<MessageInfo> _messages;
        private readonly IDisposable _messagesSubscription;

        public ChatSender(MessageSender messageSender)
        {
            _messageSender = messageSender;
            _messages = Channel.CreateBounded<MessageInfo>(1);

            _messagesSubscription = _messages.Reader
                .ReadAllAsync()
                .ToObservable()
                .SubscribeAsync(SendUpdateMessageAsync);
        }

        public Task AddMessageAsync(MessageInfo message)
        {
            return _messages.Writer.WriteAsync(message).AsTask();
        }

        private Task SendUpdateMessageAsync(MessageInfo message)
        {
            return _messageSender.SendAsync(message);
        }

        public void Dispose()
        {
            _messages.Writer.Complete();
            _messagesSubscription.Dispose();
        }
    }
}