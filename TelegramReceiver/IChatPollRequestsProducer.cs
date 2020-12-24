using Common;

namespace TelegramReceiver
{
    public interface IChatPollRequestsProducer
    {
        public void SendRequest(ChatPollRequest request);
    }
}