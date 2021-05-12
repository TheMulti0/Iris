using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScrapersDistributor
{
    public class SubscriptionsPollerService : BackgroundService
    {
        private readonly ISubscriptionsManagerClient _client;
        private readonly IConsumer<SubscriptionRequest> _consumer;
        private readonly ILogger<SubscriptionsPollerService> _logger;

        public SubscriptionsPollerService(
            ISubscriptionsManagerClient client,
            IConsumer<SubscriptionRequest> consumer,
            ILogger<SubscriptionsPollerService> logger)
        {
            _client = client;
            _consumer = consumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Polling subscriptions");

            while (true)
            {
                try
                {
                    await Poll(stoppingToken);
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to poll subscriptions, retrying in 10 seconds");

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }

        private async Task Poll(CancellationToken stoppingToken)
        {
            List<Subscription> subscriptions = await _client.Get(stoppingToken);

            foreach (Subscription subscription in subscriptions)
            {
                await _consumer.ConsumeAsync(
                    new SubscriptionRequest(SubscriptionType.Subscribe, subscription),
                    stoppingToken);
            }
        }
    }
}