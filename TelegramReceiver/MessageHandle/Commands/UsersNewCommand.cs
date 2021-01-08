using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    internal class UsersNewCommand : INewCommand
    {
        private readonly ITelegramBotClient _client;
        private readonly Update _update;
        private readonly ChatId _contextChat;
        private readonly ChatId _connectedChat;
        private readonly Language _language;
        private readonly LanguageDictionary _dictionary;
        
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly Languages _languages;

        public UsersNewCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            Languages languages)
        {
            (_client, _, _update, _contextChat, _connectedChat, _language, _dictionary) = context;
            
            _savedUsersRepository = savedUsersRepository;
            _languages = languages;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            List<SavedUser> currentUsers = _savedUsersRepository
                .GetAll()
                .Where(
                    user => user.Chats
                        .Any(chat => chat.ChatId == _connectedChat))
                .ToList();

            (InlineKeyboardMarkup markup, string text) = GetMessageDetails(currentUsers);

            if (_update.Type == UpdateType.CallbackQuery)
            {
                await _client.EditMessageTextAsync(
                    chatId: _contextChat,
                    messageId: _update.CallbackQuery.Message.MessageId,
                    text: text,
                    replyMarkup: markup,
                    cancellationToken: token);
                
            }
            else
            {
                await _client.SendTextMessageAsync(
                    chatId: _contextChat,
                    text: text,
                    replyMarkup: markup,
                    cancellationToken: token);
            }

            return new EmptyResult();
        }

        private (InlineKeyboardMarkup, string) GetMessageDetails(IReadOnlyCollection<SavedUser> currentUsers)
        {
            return currentUsers.Any() 
                ? (GetUsersMarkup(currentUsers), $"{currentUsers.Count} {_dictionary.UsersFound}") 
                : (GetNoUsersMarkup(), _dictionary.NoUsersFound);
        }

        private InlineKeyboardButton GetAddUserButton()
        {
            return InlineKeyboardButton.WithCallbackData(
                _dictionary.AddUser,
                SelectPlatformCommand.CallbackPath);
        }

        private InlineKeyboardMarkup GetNoUsersMarkup()
        {
            var buttons = new[]
            {
                new[]
                {
                    GetAddUserButton()
                }
            };

            return new InlineKeyboardMarkup(
                buttons.Concat(GetLanguageButtons()));
        }

        private InlineKeyboardMarkup GetUsersMarkup(IEnumerable<SavedUser> users)
        {
            IEnumerable<IEnumerable<InlineKeyboardButton>> userButtons = users
                .Select(UserToButton)
                .Batch(2)
                .Concat(GetLanguageButtons())
                .Concat(
                    new[]
                    {
                        new[]
                        {
                            GetAddUserButton()
                        }
                    });
            
            return new InlineKeyboardMarkup(userButtons);
        }

        private static InlineKeyboardButton UserToButton(SavedUser user)
        {
            (string userId, Platform platform) = user.User;

            return InlineKeyboardButton.WithCallbackData(
                $"{user.User}",
                $"{Route.User.ToString()}-{userId}-{Enum.GetName(platform)}");
        }

        private IEnumerable<IEnumerable<InlineKeyboardButton>> GetLanguageButtons()
        {
            InlineKeyboardButton LanguageToButton(Language language)
            {
                return
                    InlineKeyboardButton.WithCallbackData(
                        _languages.Dictionary[language].LanguageString,
                        $"{Route.SetLanguage.ToString()}-{Enum.GetName(language)}");
            }

            return Enum.GetValues<Language>()
                .Except(
                    new[]
                    {
                        _language
                    })
                .Select(LanguageToButton)
                .Batch(2);
        }
    }
}