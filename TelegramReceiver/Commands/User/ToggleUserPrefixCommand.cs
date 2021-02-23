using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class ToggleUserPrefixCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;

        public ToggleUserPrefixCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SubscriptionEntity entity = await SavedUser;
            UserChatSubscription chat = entity.Chats.First(info => info.ChatId == ConnectedChat);

            chat.ShowPrefix = !chat.ShowPrefix;
            
            await _chatSubscriptionsRepository.AddOrUpdateAsync(entity.User, chat);

            return new RedirectResult(Route.User);
        }
    }
}