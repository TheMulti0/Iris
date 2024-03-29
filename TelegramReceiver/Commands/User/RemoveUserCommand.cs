﻿using System.Threading;
using System.Threading.Tasks;
using Common;
using SubscriptionsDb;

namespace TelegramReceiver
{
    internal class RemoveUserCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;
        private readonly ISubscriptionsManager _subscriptionsManager;

        public RemoveUserCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository,
            ISubscriptionsManager subscriptionsManager): base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
            _subscriptionsManager = subscriptionsManager;
        }
        
        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            var user = await Subscription;

            await Remove(user);

            return new RedirectResult(
                Route.Subscriptions,
                Context with { SelectedPlatform = user.Platform });
        }

        private async Task Remove(SubscriptionEntity user)
        {
            var subscription = new Subscription(user.UserId, user.Platform, null);
            
            await _chatSubscriptionsRepository.RemoveAsync(user.UserId, user.Platform, ConnectedChat);

            if (! await _chatSubscriptionsRepository.ExistsAsync(user.UserId, user.Platform))
            {
                await _subscriptionsManager.Unsubscribe(subscription);                
            }
        }
    }
}