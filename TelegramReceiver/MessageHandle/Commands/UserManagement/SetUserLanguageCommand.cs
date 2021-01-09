using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class SetUserLanguageCommand : ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly Languages _languages;

        public const string CallbackPath = "setUserLanguage";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public SetUserLanguageCommand(
            ISavedUsersRepository savedUsersRepository,
            Languages languages)
        {
            _savedUsersRepository = savedUsersRepository;
            _languages = languages;
        }

        public async Task OperateAsync(Context context)
        {
            CallbackQuery query = context.Trigger.CallbackQuery;

            User user = GetUserBasicInfo(query);

            var savedUser = await _savedUsersRepository.GetAsync(user);
            UserChatInfo userChatInfo = savedUser.Chats.First(info => info.ChatId == context.ConnectedChatId);
            
            await SendRequestMessage(
                context,
                user,
                query.Message,
                userChatInfo);

            // Wait for the user to reply with desired display name
            
            var update = await context.IncomingUpdates
                .FirstAsync(u => u.Type == UpdateType.CallbackQuery && 
                                 u.CallbackQuery?.Data != ManageUserCommand.CallbackPath);

            await SetLanguage(context, user, userChatInfo, Enum.Parse<Language>(update.CallbackQuery.Data));
        }

        private async Task SetLanguage(
            Context context,
            User user,
            UserChatInfo chat,
            Language language)
        {
            chat.Language = language;
            await _savedUsersRepository.AddOrUpdateAsync(user, chat);

            await context.Client.SendTextMessageAsync(
                chatId: context.ContextChatId,
                text: $"{context.LanguageDictionary.Done}");
        }
        
        private IEnumerable<IEnumerable<InlineKeyboardButton>> GetLanguageButtons(UserChatInfo info)
        {
            InlineKeyboardButton LanguageToButton(Language language)
            {
                return
                    InlineKeyboardButton.WithCallbackData(
                        _languages.Dictionary[language].LanguageString,
                        Enum.GetName(language));
            }

            return Enum.GetValues<Language>()
                .Except(
                    new[]
                    {
                        info.Language
                    })
                .Select(LanguageToButton)
                .Batch(2);
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(items[^2], Enum.Parse<Platform>(items[^1]));
        }

        private async Task SendRequestMessage(
            Context context,
            User user,
            Message message, UserChatInfo chat)
        {
            var markup = new InlineKeyboardMarkup(
                GetLanguageButtons(chat));
            
            await context.Client.EditMessageTextAsync(
                chatId: context.ContextChatId,
                messageId: message.MessageId,
                text: context.LanguageDictionary.ChooseLanguage,
                replyMarkup: markup);
        }
    }
}