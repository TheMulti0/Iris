using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using User = Common.User;

namespace TelegramReceiver
{
    internal class UserCommand : BaseCommandd, ICommand
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

            UserChatInfo chatInfo = savedUser.Chats.First(info => info.ChatId == ConnectedChat);

            var text = GetText(chatInfo);

            var inlineKeyboardMarkup = GetMarkup(chatInfo);

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

            return new EmptyResult();
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(items[^2], Enum.Parse<Platform>(items[^1]));
        }

        private string GetText(UserChatInfo info)
        {
            var text = new StringBuilder($"{Dictionary.SettingsFor} {SelectedUser.UserId}:");
            text.AppendLine("\n");
            text.AppendLine($"<b>{Dictionary.UserId}:</b> {SelectedUser.UserId}");
            text.AppendLine($"<b>{Dictionary.Platform}:</b> {Dictionary.GetPlatform(SelectedUser.Platform)}");
            text.AppendLine($"<b>{Dictionary.DisplayName}:</b> {info.DisplayName}");
            text.AppendLine($"<b>{Dictionary.MaxDelay}:</b> {info.Interval * 2}");
            text.AppendLine($"<b>{Dictionary.Language}:</b> {_languages.Dictionary[info.Language].LanguageString}");

            string showPrefix = info.ShowPrefix ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.ShowPrefix}:</b> {showPrefix}");
            
            string showSuffix = info.ShowSuffix ? Dictionary.Enabled : Dictionary.Disabled;
            text.AppendLine($"<b>{Dictionary.ShowSuffix}:</b> {showSuffix}");
            
            return text.ToString();
        }

        private InlineKeyboardMarkup GetMarkup(UserChatInfo info)
        {
            string prefixAction = info.ShowPrefix ? Dictionary.Disable : Dictionary.Enable;
            string suffixAction = info.ShowSuffix ? Dictionary.Disable : Dictionary.Enable;

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
                            Route.Users.ToString()), 
                    }
                });
        }
    }
}