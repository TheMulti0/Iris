using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using SubscriptionsDb;

namespace TelegramReceiver
{
    internal class UserCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;
        private readonly Languages _languages;

        public UserCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository,
            Languages languages) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
            _languages = languages;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SubscriptionEntity entity = await Subscription;

            UserChatSubscription chatSubscription = entity.Chats.First(info => info.ChatId == ConnectedChat);

            string text = GetText(entity.User, chatSubscription);

            InlineKeyboardMarkup inlineKeyboardMarkup = GetMarkup(entity, chatSubscription);

            if (Trigger == null)
            {
                await Client.SendTextMessageAsync(
                    ContextChat,
                    text,
                    ParseMode.Html,
                    replyMarkup: inlineKeyboardMarkup,
                    cancellationToken: token);
            }
            else
            {
                await Client.EditMessageTextAsync(
                    ContextChat,
                    Trigger.GetMessageId(),
                    text,
                    ParseMode.Html,
                    replyMarkup: inlineKeyboardMarkup,
                    cancellationToken: token);
            }

            return new NoRedirectResult();
        }

        private string GetText(User user, UserChatSubscription subscription)
        {
            var text = new StringBuilder($"{Dictionary.SettingsFor} {user.UserId}:");
            text.AppendLine("\n");
            text.AppendLine($"<b>{Dictionary.UserId}:</b> {user.UserId}");
            text.AppendLine($"<b>{Dictionary.Platform}:</b> {Dictionary.GetPlatform(user.Platform)}");
            
            text.AppendLine("\n");
            
            text.AppendLine($"<b>{Dictionary.MaxDelay}:</b> {subscription.Interval * 2}");

            text.AppendLine("\n");
            
            text.AppendLine($"<b>{Dictionary.Prefix}:</b> {subscription.Prefix.Content}");
            text.AppendLine($"<b>{Dictionary.Suffix}:</b> {subscription.Suffix.Content}");

            text.AppendLine("\n");

            string showUrlPreview = subscription.ShowUrlPreview ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.ShowUrlPreview}:</b> {showUrlPreview}");
            
            if (SelectedPlatform != Platform.Twitter)
            {
                return text.ToString();
            }
            
            string sendScreenshotOnly = subscription.SendScreenshotOnly ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.SendScreenshotOnly}:</b> {sendScreenshotOnly}");

            return text.ToString();
        }

        private InlineKeyboardMarkup GetMarkup(
            SubscriptionEntity entity,
            UserChatSubscription subscription)
        {
            string screenshotAction = subscription.SendScreenshotOnly ? Dictionary.Disable : Dictionary.Enable;
            string showUrlAction = subscription.ShowUrlPreview ? Dictionary.Disable : Dictionary.Enable;

            IEnumerable<InlineKeyboardButton> buttons = new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.SetPrefix,
                    $"{Route.SetText}-{TextType.Prefix}-{entity.Id}"),
                
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.SetSuffix,
                    $"{Route.SetText}-{TextType.Suffix}-{entity.Id}"),
                
                InlineKeyboardButton.WithCallbackData(
                    $"{showUrlAction} {Dictionary.ShowUrlPreview}",
                    $"{Route.ToggleShowUrlPreview}-{entity.Id}"),
            };

            if (SelectedPlatform == Platform.Twitter)
            {
                buttons = buttons.Concat(
                    new [] 
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{screenshotAction} {Dictionary.SendScreenshotOnly}",
                            $"{Route.ToggleUserSendScreenshotOnly}-{entity.Id}") 
                    });
            }

            InlineKeyboardButton[] actionButtons = {
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Remove,
                    $"{Route.RemoveUser}-{entity.Id}"),
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Back,
                    $"{Route.Subscriptions}-{SelectedPlatform}")
            };
            
            return new InlineKeyboardMarkup(buttons.Concat(actionButtons).Batch(1));
        }
    }
}