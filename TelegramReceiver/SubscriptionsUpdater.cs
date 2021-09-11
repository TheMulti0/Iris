using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;
using SubscriptionsDb;

namespace TelegramReceiver
{
    public class SubscriptionsUpdater : BackgroundService
    {
        private readonly IChatSubscriptionsRepository _repository;
        private readonly ISubscriptionsManager _manager;

        public SubscriptionsUpdater(
            IChatSubscriptionsRepository repository,
            ISubscriptionsManager manager)
        {
            _repository = repository;
            _manager = manager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (SubscriptionEntity subscriptionEntity in _repository.Get())
            {
                TimeSpan timeSpan = subscriptionEntity.Chats.Min(subscription => subscription.Interval);

                await _manager.Subscribe(
                    new Subscription(subscriptionEntity.UserId, subscriptionEntity.Platform, timeSpan),
                    stoppingToken);
            }
        }
    }
}