using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using SubscriptionsDb;

namespace TelegramReceiver
{
    internal class ToggleTextModeCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;

        public ToggleTextModeCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SubscriptionEntity entity = await Subscription;
            UserChatSubscription chat = entity.Chats.First(info => info.ChatInfo.Id == ConnectedChat);

            List<TextMode> modes = Enum.GetValues<TextMode>().ToList();
            
            var mode = GetTextType() == TextType.Prefix 
                ? chat.Prefix.Mode 
                : chat.Suffix.Mode;

            mode = (int) mode == modes.Count - 1 
                ? modes.FirstOrDefault() 
                : modes[modes.IndexOf(mode) + 1];

            if (GetTextType() == TextType.Prefix)
            {
                chat.Prefix.Mode = mode;
            }
            else
            {
                chat.Suffix.Mode = mode;
            }
            
            await _chatSubscriptionsRepository.AddOrUpdateAsync(entity.User, chat);

            return new RedirectResult(Route.SetText);
        }
    }
}