using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace SubscriptionsManager
{
    public class ChatSubscriptionRequestsConsumer : IConsumer<ChatSubscriptionRequest>
    {
        private readonly IProducer<ChatSubscriptionRequest> _producer;
        private readonly ILogger<ChatSubscriptionRequestsConsumer> _logger;

        public ChatSubscriptionRequestsConsumer(
            IProducer<SubscriptionRequest> producer,
            ILogger<ChatSubscriptionRequestsConsumer> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task ConsumeAsync(ChatSubscriptionRequest subscriptionRequest, CancellationToken token)
        {
            _logger.LogInformation("Received chat subscription request {}", subscriptionRequest);
            
            _producer.Send(subscriptionRequest);
        }
    }
}