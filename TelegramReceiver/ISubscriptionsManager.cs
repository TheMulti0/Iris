using System.Threading;
using System.Threading.Tasks;
using Common;

namespace TelegramReceiver
{
    public interface ISubscriptionsManager
    {
        Task Subscribe(Subscription subscription, CancellationToken ct = default);
        
        Task Unsubscribe(Subscription subscription, CancellationToken ct = default);
    }
}