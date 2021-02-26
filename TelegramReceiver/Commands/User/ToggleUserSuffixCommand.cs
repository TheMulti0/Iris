using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using SubscriptionsDataLayer;

namespace TelegramReceiver
{
    internal class ToggleUserSuffixCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;

        public ToggleUserSuffixCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SubscriptionEntity entity = await Subscription;
            UserChatSubscription chat = entity.Chats.First(info => info.ChatId == ConnectedChat);

            chat.ShowSuffix = !chat.ShowSuffix;
            
            await _chatSubscriptionsRepository.AddOrUpdateAsync(entity.User, chat);

            return new RedirectResult(Route.User);
        }
    }
}