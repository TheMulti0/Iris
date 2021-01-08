using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class UserNewCommand : INewCommand
    {
        private readonly ITelegramBotClient _client;
        private readonly Update _update;
        private readonly ChatId _contextChat;
        private readonly ChatId _connectedChat;
        private readonly Language _language;
        private readonly LanguageDictionary _dictionary;
        private readonly User _user;
        
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly Languages _languages;

        public UserNewCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            Languages languages)
        {
            (_client, _, _update, _contextChat, _connectedChat, _language, _dictionary) = context;

            _user = context.SelectedSavedUser ?? GetUserBasicInfo(_update.CallbackQuery);
            
            _savedUsersRepository = savedUsersRepository;
            _languages = languages;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            var savedUser = await _savedUsersRepository.GetAsync(_user);

            UserChatInfo chatInfo = savedUser.Chats.First(info => info.ChatId == _connectedChat);
            
            var text = GetText(chatInfo);

            var inlineKeyboardMarkup = GetMarkup(chatInfo);
            
            await _client.EditMessageTextAsync(
                chatId: _contextChat,
                messageId: _update.GetMessageId(),
                text: text,
                parseMode: ParseMode.Html,
                replyMarkup: inlineKeyboardMarkup,
                cancellationToken: token);

            return new EmptyResult();
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(items[^2], Enum.Parse<Platform>(items[^1]));
        }

        private string GetText(UserChatInfo info)
        {
            var text = new StringBuilder($"{_dictionary.SettingsFor} {_user}:");
            text.AppendLine("\n");
            text.AppendLine($"<b>{_dictionary.UserId}:</b> {_user.UserId}");
            text.AppendLine($"<b>{_dictionary.Platform}:</b> {_user.Platform}");
            text.AppendLine($"<b>{_dictionary.DisplayName}:</b> {info.DisplayName}");
            text.AppendLine($"<b>{_dictionary.MaxDelay}:</b> {info.Interval * 2}");
            text.AppendLine($"<b>{_dictionary.Language}:</b> {_languages.Dictionary[info.Language].LanguageString}");

            string showPrefix = info.ShowPrefix ? _dictionary.Enabled : _dictionary.Disabled;
            text.AppendLine($"<b>{_dictionary.ShowPrefix}:</b> {showPrefix}");
            
            string showSuffix = info.ShowSuffix ? _dictionary.Enabled : _dictionary.Disabled;
            text.AppendLine($"<b>{_dictionary.ShowSuffix}:</b> {showSuffix}");
            
            return text.ToString();
        }

        private InlineKeyboardMarkup GetMarkup(UserChatInfo info)
        {
            var showPrefixPath = info.ShowPrefix
                ? DisablePrefixCommand.CallbackPath
                : EnablePrefixCommand.CallbackPath;
            
            var showSuffixPath = info.ShowPrefix
                ? DisableSuffixCommand.CallbackPath
                : EnableSuffixCommand.CallbackPath;

            string prefixAction = info.ShowPrefix ? _dictionary.Disable : _dictionary.Enable;
            string suffixAction = info.ShowSuffix ? _dictionary.Disable : _dictionary.Enable;

            string userInfo = $"{_user.UserId}-{_user.Platform}";
            
            return new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            _dictionary.SetDisplayName,
                            $"{SetUserDisplayNameCommand.CallbackPath}-{userInfo}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            _dictionary.SetLanguage,
                            $"{SetUserLanguageCommand.CallbackPath}-{userInfo}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{prefixAction} {_dictionary.ShowPrefix}",
                            $"{showPrefixPath}-{userInfo}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{suffixAction} {_dictionary.ShowSuffix}",
                            $"{showSuffixPath}-{userInfo}")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            _dictionary.Remove,
                            $"{RemoveUserCommand.CallbackPath}-{userInfo}"),                        
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            _dictionary.Back,
                            Route.Users.ToString()), 
                    }
                });
        }
    }
}