using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Extensions;

namespace TelegramConsumer
{
    internal class ChatSender
    {
        private readonly MessageSender _messageSender;
        private readonly SemaphoreSlim _lock;
        private readonly Channel<MessageInfo> _messages;
        private readonly IDisposable _messagesSubscription;

        public ChatSender(MessageSender messageSender)
        {
            _messageSender = messageSender;

            const int capacity = 1;
            _lock = new SemaphoreSlim(capacity);
            _messages = Channel.CreateBounded<MessageInfo>(capacity);

            _messagesSubscription = _messages.Reader.ReadAllAsync()
                .ToObservable()
                .SubscribeAsync(SendMessageAsync, OnComplete);
        }

        public Task AddMessageAsync(MessageInfo message)
        {
            return _messages.Writer.WriteAsync(message).AsTask();
        }

        private Task SendMessageAsync(MessageInfo message)
        {
            return _messageSender.SendAsync(message);
        }

        private void OnComplete()
        {
            _lock.Release();
        }

        public async ValueTask WaitForCompleteAsync()
        {
            await _lock.WaitAsync();
            _messagesSubscription.Dispose();
        }

        public void Cancel()
        {
            _messages.Writer.Complete();
            _messagesSubscription.Dispose();
        }
    }
}