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
using Telegram.Bot.Exceptions;

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
            try
            {
                foreach (SubscriptionEntity subscriptionEntity in _repository.Get())
                {
                    await foreach (UserChatSubscription subscription in InsertChat(subscriptionEntity, stoppingToken))
                    {
                        if (subscription.ChatInfo != null)
                        {
                            await _repository.AddOrUpdateAsync(subscriptionEntity.User, subscription);
                    
                            Console.WriteLine($"Migrated user chat subscription {subscription.ChatInfo.Id}");    
                        }
                        else
                        {
                            //await _repository.RemoveAsync(subscriptionEntity.User, subscription.ChatId);
                            
                            //Console.WriteLine($"Removed user chat subscription {subscription.ChatId}");
                        }
                        
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async IAsyncEnumerable<UserChatSubscription> InsertChat(
            SubscriptionEntity subscriptionEntity,
            [EnumeratorCancellation] CancellationToken stoppingToken)
        {
            foreach (UserChatSubscription subscription in subscriptionEntity.Chats)
            {
                ChatInfo? chatInfo = null;

                try
                {
                    if (subscription.ChatInfo != null)
                    {
                        continue;
                    }
                    
                    Telegram.Bot.Types.Chat chat = await _client.GetChatAsync(subscription.ChatId, stoppingToken);

                    chatInfo = JsonSerializer.Deserialize<ChatInfo>(JsonSerializer.Serialize(chat));
                }
                catch (ChatNotFoundException)
                {
                }

                yield return subscription with { ChatInfo = chatInfo };
            }
        }
    }
}