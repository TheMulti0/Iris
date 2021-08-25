using System.Threading.Tasks;
using Common;

namespace TelegramReceiver
{
    public interface ISubscriptionsManager
    {
        Task Subscribe(Subscription subscription);
        
        Task Unsubscribe(Subscription subscription);
    }
}