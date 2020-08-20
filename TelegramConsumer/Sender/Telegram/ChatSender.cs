using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Extensions;

namespace TelegramConsumer
{
    internal class ChatSender
    {
        private readonly MessageSender _messageSender;
        private readonly Channel<MessageInfo> _messages;

        public ChatSender(MessageSender messageSender)
        {
            _messageSender = messageSender;
            _messages = Channel.CreateBounded<MessageInfo>(1);

            _messages.Reader.ReadAllAsync().ToObservable().SubscribeAsync(SendUpdateMessageAsync);
        }

        public Task AddMessageAsync(MessageInfo message)
        {
            return _messages.Writer.WriteAsync(message).AsTask();
        }

        public void Stop()
        {
            _messages.Writer.Complete();
        }

        private Task SendUpdateMessageAsync(MessageInfo message)
        {
            return _messageSender.SendAsync(message);
        }
    }
}