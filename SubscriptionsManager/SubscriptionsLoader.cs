using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Hosting;
using SubscriptionsDb;

namespace SubscriptionsManager
{
    public class SubscriptionsLoader : BackgroundService
    {
        private readonly IChatSubscriptionsRepository _repository;
        private readonly IConsumer<ChatSubscriptionRequest> _consumer;

        public SubscriptionsLoader(
            IChatSubscriptionsRepository repository,
            IConsumer<ChatSubscriptionRequest> consumer)
        {
            _repository = repository;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (SubscriptionEntity entity in _repository.Get())
            {
                foreach (UserChatSubscription chat in entity.Chats)
                {
                    var subscription = new Subscription(
                        entity.User,
                        chat.Interval,
                        DateTime.Now);

                    var request = new ChatSubscriptionRequest(
                        SubscriptionType.Subscribe,
                        subscription,
                        chat.ChatInfo.Id);
                        
                    await _consumer.ConsumeAsync(request, stoppingToken);
                }
            }
        }
    }
}