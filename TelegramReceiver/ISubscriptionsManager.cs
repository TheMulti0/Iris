using System.Threading.Tasks;
using Common;

namespace TelegramReceiver
{
    public interface ISubscriptionsManager
    {
        Task Subscribe(Subscription subscription, long chatId);
        
        Task Unsubscribe(Subscription subscription, long chatId);
    }
}