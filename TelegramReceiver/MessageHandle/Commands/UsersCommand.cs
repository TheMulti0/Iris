using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramReceiver.Data;
using UserDataLayer;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    internal class UsersCommand : ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly Languages _languages;

        public const string CallbackPath = "home";
        
        public ITrigger[] Triggers { get; } = {
            new MessageTextTrigger("/start"),
            new CallbackTrigger(CallbackPath)
        };

        public UsersCommand(
            ISavedUsersRepository savedUsersRepository,
            Languages languages)
        {
            _savedUsersRepository = savedUsersRepository;
            _languages = languages;
        }

        public async Task OperateAsync(Context context)
        {
            List<SavedUser> currentUsers = _savedUsersRepository
                .GetAll()
                .Where(
                    user => user.Chats
                        .Any(chat => chat.ChatId == context.ConnectedChatId))
                .ToList();

            (InlineKeyboardMarkup markup, string text) = Get(context, currentUsers);

            if (context.Update.Type == UpdateType.CallbackQuery)
            {
                await context.Client.EditMessageTextAsync(
                    chatId: context.ContextChatId,
                    messageId: context.Update.CallbackQuery.Message.MessageId,
                    text: text,
                    replyMarkup: markup);
                return;
            }
            
            await context.Client.SendTextMessageAsync(
                chatId: context.ContextChatId,
                text: text,
                replyMarkup: markup);
    }

        private (InlineKeyboardMarkup, string) Get(Context context, IReadOnlyCollection<SavedUser> currentUsers)
        {
            return currentUsers.Any() 
                ? (
                    GetUsersMarkup(context, currentUsers),
                    $"{currentUsers.Count} {context.LanguageDictionary.UsersFound}") 
                : (
                    GetNoUsersMarkup(context),
                    context.LanguageDictionary.NoUsersFound);
        }

        private static InlineKeyboardButton GetAddUserButton(Context context)
        {
            return InlineKeyboardButton.WithCallbackData(
                context.LanguageDictionary.AddUser,
                SelectPlatformCommand.CallbackPath);
        }

        private InlineKeyboardMarkup GetNoUsersMarkup(Context context)
        {
            var buttons = new[]
            {
                new[]
                {
                    GetAddUserButton(context)
                }
            };

            return new InlineKeyboardMarkup(
                buttons.Concat(GetLanguageButtons(context)));
        }

        private InlineKeyboardMarkup GetUsersMarkup(Context context, IEnumerable<SavedUser> users)
        {
            IEnumerable<IEnumerable<InlineKeyboardButton>> userButtons = users
                .Select(UserToButton)
                .Batch(2)
                .Concat(GetLanguageButtons(context))
                .Concat(
                    new[]
                    {
                        new[]
                        {
                            GetAddUserButton(context)
                        }
                    });
            
            return new InlineKeyboardMarkup(userButtons);
        }

        private static InlineKeyboardButton UserToButton(SavedUser user)
        {
            (string userId, Platform platform) = user.User;

            return InlineKeyboardButton.WithCallbackData(
                $"{user.User}",
                $"{ManageUserCommand.CallbackPath}-{userId}-{Enum.GetName(platform)}");
        }

        private IEnumerable<IEnumerable<InlineKeyboardButton>> GetLanguageButtons(Context context)
        {
            InlineKeyboardButton LanguageToButton(Language language)
            {
                return
                    InlineKeyboardButton.WithCallbackData(
                        _languages.Dictionary[language].LanguageString,
                        $"{SetLanguageCommand.CallbackPath}-{Enum.GetName(language)}");
            }

            return Enum.GetValues<Language>()
                .Except(
                    new[]
                    {
                        context.Language
                    })
                .Select(LanguageToButton)
                .Batch(2);
        }
    }
}