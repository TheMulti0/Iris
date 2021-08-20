using System.Threading.Tasks;
using Common;
using Extensions;

namespace TelegramReceiver
{
    public class OldSubscriptionsManager : ISubscriptionsManager
    {
        private readonly IProducer<ChatSubscriptionRequest> _producer;

        public OldSubscriptionsManager(IProducer<ChatSubscriptionRequest> producer)
        {
            _producer = producer;
        }

        public Task Subscribe(Subscription subscription, long chatId)
        {
            _producer.Send(
                new ChatSubscriptionRequest(
                    SubscriptionType.Subscribe,
                    subscription,
                    chatId));
            
            return Task.CompletedTask;
        }

        public Task Unsubscribe(Subscription subscription, long chatId)
        {
            _producer.Send(
                new ChatSubscriptionRequest(
                    SubscriptionType.Unsubscribe,
                    subscription,
                    chatId));

            return Task.CompletedTask;
        }
    }
}