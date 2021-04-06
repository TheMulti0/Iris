using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using SubscriptionsDb;

namespace TelegramReceiver
{
    internal class ToggleTextCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;

        public ToggleTextCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SubscriptionEntity entity = await Subscription;
            UserChatSubscription chat = entity.Chats.First(info => info.ChatInfo.Id == ConnectedChat);

            if (GetTextType() == TextType.Prefix)
            {
                chat.Prefix.Enabled = !chat.Prefix.Enabled;
            }
            else
            {
                chat.Suffix.Enabled = !chat.Suffix.Enabled;
            }
            
            await _chatSubscriptionsRepository.AddOrUpdateAsync(entity.User, chat);

            return new RedirectResult(Route.SetText);
        }
    }
}