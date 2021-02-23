using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using UserDataLayer;
using User = Common.User;

namespace TelegramReceiver
{
    internal class RemoveUserCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;
        private readonly IProducer<ChatSubscriptionRequest> _producer;

        public RemoveUserCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository,
            IProducer<ChatSubscriptionRequest> producer): base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
            _producer = producer;
        }
        
        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            var user = (await SavedUser).User;

            await Remove(user);

            return new RedirectResult(
                Route.Subscriptions,
                Context with { SelectedPlatform = user.Platform });
        }

        private async Task Remove(User user)
        {
            var userPollRule = new Subscription(user, null);
            
            await _chatSubscriptionsRepository.RemoveAsync(user, ConnectedChat);

            if (! await _chatSubscriptionsRepository.ExistsAsync(user))
            {
                _producer.Send(
                    new ChatSubscriptionRequest(
                        SubscriptionType.Unsubscribe,
                        userPollRule,
                        ConnectedChat));                
            }
        }
    }
}