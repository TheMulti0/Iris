using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using PostsListener.Client;

namespace TelegramReceiver
{
    public class SubscriptionsManager : ISubscriptionsManager
    {
        private readonly INewPostSubscriptionsClient _client;
        private readonly bool _subscribeToOldPosts;

        public SubscriptionsManager(
            INewPostSubscriptionsClient client,
            bool subscribeToOldPosts)
        {
            _client = client;
            _subscribeToOldPosts = subscribeToOldPosts;
        }

        public async Task Subscribe(Subscription subscription, CancellationToken ct = default)
        {
            if (subscription.Interval == null)
            {
                throw new NullReferenceException(nameof(subscription.Interval));
            }

            DateTime earliestPostDate = _subscribeToOldPosts 
                ? DateTime.MinValue 
                : DateTime.Now;
            
            await _client.AddOrUpdateSubscription(
                subscription.UserId,
                subscription.Platform,
                (TimeSpan) subscription.Interval,
                earliestPostDate,
                ct);
        }

        public async Task Unsubscribe(Subscription subscription, CancellationToken ct = default)
        {
            await _client.RemoveSubscription(subscription.UserId, subscription.Platform, ct);
        }
    }
}