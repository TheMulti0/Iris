using Common;

namespace MessagesManager
{
    public interface IMessagesProducer
    {
        public void SendMessage(Message message);
    }
}