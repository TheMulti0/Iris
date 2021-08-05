using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using Scraper.RabbitMq.Client;

namespace ScrapersDistributor
{
    internal class SubscriptionRequestsConsumer : IConsumer<SubscriptionRequest>
    {
        private readonly IScraperRabbitMqClient _client;
        private readonly ILogger<SubscriptionRequestsConsumer> _logger;

        public SubscriptionRequestsConsumer(
            IScraperRabbitMqClient client,
            ILogger<SubscriptionRequestsConsumer> logger)
        {
            _client = client;
            _logger = logger;
        }
        
        public async Task ConsumeAsync(
            SubscriptionRequest request,
            CancellationToken token)
        {
            try
            {
                _logger.LogInformation("Received {}", request);
                
                (SubscriptionType type, Subscription rule) = request;
                
                if (type == SubscriptionType.Subscribe)
                {
                    await AddUserSubscription(rule);
                }
                else
                {
                    await RemoveUserSubscription(rule);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }
        }

        private async Task AddUserSubscription(Subscription subscription)
        {
            _logger.LogInformation("Adding user subscription {}", subscription);

            string platform = subscription.User.Platform.ToString().ToLower();
            await _client.SubscribeAsync(platform, subscription.User.UserId, (TimeSpan) subscription.Interval);
        }

        private async Task RemoveUserSubscription(Subscription subscription)
        {
            _logger.LogInformation("Removing user subscription {}", subscription);
            
            string platform = subscription.User.Platform.ToString().ToLower();
            await _client.UnsubscribeAsync(platform, subscription.User.UserId);
        }
    }
}