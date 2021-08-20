using System;
using System.Threading.Tasks;
using Common;
using Scraper.RabbitMq.Client;

namespace TelegramReceiver
{
    public class NewSubscriptionsManager : ISubscriptionsManager
    {
        private readonly INewPostSubscriptionsClient _client;

        public NewSubscriptionsManager(INewPostSubscriptionsClient client)
        {
            _client = client;
        }

        public async Task Subscribe(Subscription subscription, long chatId)
        {
            string platform = GetPlatform(subscription);
            await _client.AddOrUpdateSubscription(subscription.User.UserId, platform, (TimeSpan)subscription.Interval);
        }

        public async Task Unsubscribe(Subscription subscription, long chatId)
        {
            string platform = GetPlatform(subscription);
            await _client.RemoveSubscription(subscription.User.UserId, platform);
        }

        private static string GetPlatform(Subscription subscription) => subscription.User.Platform.ToString().ToLower();
    }
}