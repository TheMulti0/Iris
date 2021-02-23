using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class ToggleUserSendScreenshotOnlyCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;

        public ToggleUserSendScreenshotOnlyCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SubscriptionEntity entity = await SavedUser;
            UserChatSubscription chat = entity.Chats.First(info => info.ChatId == ConnectedChat);

            chat.SendScreenshotOnly = !chat.SendScreenshotOnly;
            
            await _chatSubscriptionsRepository.AddOrUpdateAsync(entity.User, chat);

            return new RedirectResult(Route.User);
        }
    }
}