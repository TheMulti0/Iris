using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;

namespace SubscriptionsDataLayer.Migrator
{
    public class PrefixSuffixMigrator : BackgroundService
    {
        private IChatSubscriptionsRepository _repository;
        private readonly Languages _languages;

        public PrefixSuffixMigrator(IChatSubscriptionsRepository repository, Languages languages)
        {
            _repository = repository;
            _languages = languages;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // IAsyncEnumerable<SubscriptionEntity> subscriptions = _repository.Get().ToAsyncEnumerable();
            //
            // await foreach (SubscriptionEntity subscription in subscriptions.WithCancellation(stoppingToken))
            // {
            //     foreach (UserChatSubscription chatSubscription in subscription.Chats)
            //     {
            //         var lang = _languages.Dictionary[chatSubscription.Language];
            //         
            //         chatSubscription.Prefix = new Text
            //         {
            //             Enabled = true,
            //             Content = $"{chatSubscription.DisplayName} ({lang.GetPlatform(subscription.User.Platform)}):",
            //             Mode = TextMode.HyperlinkedText
            //         };
            //
            //         if (chatSubscription.ShowSuffix)
            //         {
            //             chatSubscription.Suffix = new Text
            //             {
            //                 Enabled = true,
            //                 Content = string.Empty,
            //                 Mode = TextMode.Url
            //             };
            //         }
            //         else
            //         {
            //             chatSubscription.Suffix = new Text
            //             {
            //                 Enabled = false
            //             };
            //         }
            //
            //         await _repository.AddOrUpdateAsync(subscription.User, chatSubscription);
            //     }
            // }
        }
    }
}