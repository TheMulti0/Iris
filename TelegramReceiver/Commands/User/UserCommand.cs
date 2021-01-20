using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
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
            var savedUser = await _savedUsersRepository.GetAsync(SelectedUser);

            UserChatSubscription chatSubscription = savedUser.Chats.First(info => info.ChatId == ConnectedChat);

            var text = GetText(chatSubscription);

            var inlineKeyboardMarkup = GetMarkup(chatSubscription);

            if (Trigger == null)
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: text,
                    parseMode: ParseMode.Html,
                    replyMarkup: inlineKeyboardMarkup,
                    cancellationToken: token);
            }
            else
            {
                await Client.EditMessageTextAsync(
                    chatId: ContextChat,
                    messageId: Trigger.GetMessageId(),
                    text: text,
                    parseMode: ParseMode.Html,
                    replyMarkup: inlineKeyboardMarkup,
                    cancellationToken: token);
            }

            return new NoRedirectResult();
        }

        private string GetText(UserChatSubscription subscription)
        {
            var text = new StringBuilder($"{Dictionary.SettingsFor} {subscription.DisplayName}:");
            text.AppendLine("\n");
            text.AppendLine($"<b>{Dictionary.UserId}:</b> {SelectedUser.UserId}");
            text.AppendLine($"<b>{Dictionary.Platform}:</b> {Dictionary.GetPlatform(SelectedUser.Platform)}");
            text.AppendLine($"<b>{Dictionary.DisplayName}:</b> {subscription.DisplayName}");
            text.AppendLine($"<b>{Dictionary.MaxDelay}:</b> {subscription.Interval * 2}");
            text.AppendLine($"<b>{Dictionary.Language}:</b> {_languages.Dictionary[subscription.Language].LanguageString}");

            string showPrefix = subscription.ShowPrefix ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.ShowPrefix}:</b> {showPrefix}");
            
            string showSuffix = subscription.ShowSuffix ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.ShowSuffix}:</b> {showSuffix}");
            
            string sendScreenshotOnly = subscription.SendScreenshotOnly ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.SendScreenshotOnly}:</b> {sendScreenshotOnly}");
            
            return text.ToString();
        }

        private InlineKeyboardMarkup GetMarkup(UserChatSubscription subscription)
        {
            string prefixAction = subscription.ShowPrefix ? Dictionary.Disable : Dictionary.Enable;
            string suffixAction = subscription.ShowSuffix ? Dictionary.Disable : Dictionary.Enable;
            string screenshotAction = subscription.SendScreenshotOnly ? Dictionary.Disable : Dictionary.Enable;

            string userInfo = $"{SelectedUser.UserId}-{SelectedUser.Platform}";
            
            return new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            Dictionary.SetDisplayName,
                            $"{Route.SetUserDisplayName}-{userInfo}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            Dictionary.SetLanguage,
                            $"{Route.SetUserLanguage}-{userInfo}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{prefixAction} {Dictionary.ShowPrefix}",
                            $"{Route.ToggleUserPrefix}-{userInfo}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{suffixAction} {Dictionary.ShowSuffix}",
                            $"{Route.ToggleUserSuffix}-{userInfo}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{screenshotAction} {Dictionary.SendScreenshotOnly}",
                            $"{Route.ToggleUserSendScreenshotOnly}-{userInfo}")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            Dictionary.Remove,
                            $"{Route.RemoveUser}-{userInfo}"),                        
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            Dictionary.Back,
                            $"{Route.Subscriptions}-{SelectedPlatform}") 
                    }
                });
        }
    }
}