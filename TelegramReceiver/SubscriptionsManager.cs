﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using PostsListener.Client;

namespace TelegramReceiver
{
    public class SubscriptionsManager : ISubscriptionsManager
    {
        private readonly INewPostSubscriptionsClient _client;

        public SubscriptionsManager(INewPostSubscriptionsClient client)
        {
            _client = client;
        }

        public async Task Subscribe(Subscription subscription, CancellationToken ct = default)
        {
            if (subscription.Interval == null)
            {
                throw new NullReferenceException(nameof(subscription.Interval));
            }
            
            await _client.AddOrUpdateSubscription(
                subscription.UserId,
                subscription.Platform,
                (TimeSpan) subscription.Interval,
                DateTime.Now,
                ct);
        }

        public async Task Unsubscribe(Subscription subscription, CancellationToken ct = default)
        {
            await _client.RemoveSubscription(subscription.UserId, subscription.Platform, ct);
        }
    }
}