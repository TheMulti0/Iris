using System.Threading;
using System.Threading.Tasks;
using Common;
using SubscriptionsDb;
using User = Common.User;

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
            var user = (await Subscription).User;

            await Remove(user);

            return new RedirectResult(
                Route.Subscriptions,
                Context with { SelectedPlatform = user.Platform });
        }

        private async Task Remove(User user)
        {
            var subscription = new Subscription(user, null);
            
            await _chatSubscriptionsRepository.RemoveAsync(user, ConnectedChat);

            if (! await _chatSubscriptionsRepository.ExistsAsync(user))
            {
                await _subscriptionsManager.Unsubscribe(subscription, ConnectedChat);                
            }
        }
    }
}