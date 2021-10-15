using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using SubscriptionsDb;

namespace TelegramReceiver
{
    internal class ToggleTextStyleCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;

        public ToggleTextStyleCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SubscriptionEntity entity = await Subscription;
            UserChatSubscription chat = entity.Chats.First(info => info.ChatInfo.Id == ConnectedChat);

            List<TextStyle> styles = Enum.GetValues<TextStyle>().ToList();
            
            var style = GetTextType() == TextType.Prefix 
                ? chat.Prefix.Style 
                : chat.Suffix.Style;

            style = (int) style == styles.Count - 1 
                ? styles.FirstOrDefault() 
                : styles[styles.IndexOf(style) + 1];

            if (GetTextType() == TextType.Prefix)
            {
                chat.Prefix.Style = style;
            }
            else
            {
                chat.Suffix.Style = style;
            }
            
            await _chatSubscriptionsRepository.AddOrUpdateAsync(entity.UserId, entity.Platform, chat);

            return new RedirectResult(Route.SetText);
        }
    }
}