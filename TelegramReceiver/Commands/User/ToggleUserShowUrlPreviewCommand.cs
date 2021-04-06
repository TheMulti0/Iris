using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using SubscriptionsDb;

namespace TelegramReceiver
{
    internal class ToggleUserShowUrlPreviewCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;

        public ToggleUserShowUrlPreviewCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SubscriptionEntity entity = await Subscription;
            UserChatSubscription chat = entity.Chats.First(info => info.ChatInfo.Id == ConnectedChat);

            chat.ShowUrlPreview = !chat.ShowUrlPreview;
            
            await _chatSubscriptionsRepository.AddOrUpdateAsync(entity.User, chat);

            return new RedirectResult(Route.User);
        }
    }
}