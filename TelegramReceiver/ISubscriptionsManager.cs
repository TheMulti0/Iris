using Common;

namespace TelegramReceiver
{
    public interface ISubscriptionsManager
    {
        void Subscribe(Subscription subscription, long chatId);
        
        void Unsubscribe(Subscription subscription, long chatId);
    }
}