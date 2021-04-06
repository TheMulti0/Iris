using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace SubscriptionsDb.Migrator
{
    public class SubscriptionsDbMigrator : BackgroundService
    {
        private readonly TelegramBotClient _client;
        private readonly IChatSubscriptionsRepository _repository;

        public SubscriptionsDbMigrator(TelegramBotClient client, IChatSubscriptionsRepository repository)
        {
            _client = client;
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (SubscriptionEntity subscriptionEntity in _repository.Get())
            {
                await foreach (UserChatSubscription subscription in InsertChat(subscriptionEntity, stoppingToken))
                {
                    await _repository.AddOrUpdateAsync(subscriptionEntity.User, subscription);
                }
            }
        }

        private async IAsyncEnumerable<UserChatSubscription> InsertChat(
            SubscriptionEntity subscriptionEntity,
            [EnumeratorCancellation] CancellationToken stoppingToken)
        {
            foreach (UserChatSubscription subscription in subscriptionEntity.Chats)
            {
                Telegram.Bot.Types.Chat chat = await _client.GetChatAsync(subscription.ChatId, stoppingToken);

                var chatInfo = JsonSerializer.Deserialize<ChatInfo>(JsonSerializer.Serialize(chat));

                yield return subscription with { ChatInfo = chatInfo };
            }
        }
    }
}