using Common;
using Extensions;

namespace TelegramReceiver
{
    public class SubscriptionsManager : ISubscriptionsManager
    {
        private readonly IProducer<ChatSubscriptionRequest> _producer;

        public SubscriptionsManager(IProducer<ChatSubscriptionRequest> producer)
        {
            _producer = producer;
        }

        public void Subscribe(Subscription subscription, long chatId)
        {
            _producer.Send(
                new ChatSubscriptionRequest(
                    SubscriptionType.Subscribe,
                    subscription,
                    chatId));
        }

        public void Unsubscribe(Subscription subscription, long chatId)
        {
            _producer.Send(
                new ChatSubscriptionRequest(
                    SubscriptionType.Unsubscribe,
                    subscription,
                    chatId));
        }
    }
}