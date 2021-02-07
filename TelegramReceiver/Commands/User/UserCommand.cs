using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class UserCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly Languages _languages;

        public UserCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            Languages languages) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
            _languages = languages;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SavedUser savedUser = await SavedUser;

            UserChatSubscription chatSubscription = savedUser.Chats.First(info => info.ChatId == ConnectedChat);

            string text = GetText(savedUser.User, chatSubscription);

            InlineKeyboardMarkup inlineKeyboardMarkup = GetMarkup(savedUser, chatSubscription);

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
            var text = new StringBuilder($"{Dictionary.SettingsFor} {subscription.DisplayName}:");
            text.AppendLine("\n");
            text.AppendLine($"<b>{Dictionary.UserId}:</b> {user.UserId}");
            text.AppendLine($"<b>{Dictionary.Platform}:</b> {Dictionary.GetPlatform(user.Platform)}");
            text.AppendLine($"<b>{Dictionary.DisplayName}:</b> {subscription.DisplayName}");
            text.AppendLine($"<b>{Dictionary.MaxDelay}:</b> {subscription.Interval * 2}");
            text.AppendLine(
                $"<b>{Dictionary.Language}:</b> {_languages.Dictionary[subscription.Language].LanguageString}");

            string showPrefix = subscription.ShowPrefix ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.ShowPrefix}:</b> {showPrefix}");

            string showSuffix = subscription.ShowSuffix ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.ShowSuffix}:</b> {showSuffix}");

            if (SelectedPlatform != Platform.Twitter)
            {
                return text.ToString();
            }

            string sendScreenshotOnly = subscription.SendScreenshotOnly ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.SendScreenshotOnly}:</b> {sendScreenshotOnly}");

            return text.ToString();
        }

        private InlineKeyboardMarkup GetMarkup(
            SavedUser savedUser,
            UserChatSubscription subscription)
        {
            string prefixAction = subscription.ShowPrefix ? Dictionary.Disable : Dictionary.Enable;
            string suffixAction = subscription.ShowSuffix ? Dictionary.Disable : Dictionary.Enable;
            string screenshotAction = subscription.SendScreenshotOnly ? Dictionary.Disable : Dictionary.Enable;

            IEnumerable<InlineKeyboardButton> buttons = new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.SetDisplayName,
                    $"{Route.SetUserDisplayName}-{savedUser.Id}"),
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.SetLanguage,
                    $"{Route.SetUserLanguage}-{savedUser.Id}"),
                InlineKeyboardButton.WithCallbackData(
                    $"{prefixAction} {Dictionary.ShowPrefix}",
                    $"{Route.ToggleUserPrefix}-{savedUser.Id}"),
                InlineKeyboardButton.WithCallbackData(
                    $"{suffixAction} {Dictionary.ShowSuffix}",
                    $"{Route.ToggleUserSuffix}-{savedUser.Id}"),
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Remove,
                    $"{Route.RemoveUser}-{savedUser.Id}"),
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Back,
                    $"{Route.Subscriptions}-{SelectedPlatform}")
            };

            if (SelectedPlatform == Platform.Twitter)
            {
                buttons = buttons.Concat(
                    new [] 
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{screenshotAction} {Dictionary.SendScreenshotOnly}",
                            $"{Route.ToggleUserSendScreenshotOnly}-{savedUser.Id}") 
                    });
            }
            
            return new InlineKeyboardMarkup(buttons.Batch(1));
        }
    }
}