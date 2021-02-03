using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using User = Common.User;

namespace TelegramReceiver
{
    internal class SetUserLanguageCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly Languages _languages;

        public SetUserLanguageCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            Languages languages) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
            _languages = languages;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            CallbackQuery query = Trigger.CallbackQuery;
            var savedUser = await SavedUser;
            
            UserChatSubscription userChatSubscription = savedUser.Chats.First(info => info.ChatId == ConnectedChat);
            
            await SendRequestMessage(
                query.Message,
                savedUser,
                userChatSubscription);

            // Wait for the user to reply with desired display name
            
            var update = await GetNextCallbackQuery();

            if (update == null || update.CallbackQuery.Data.StartsWith(Route.User.ToString()))
            {
                return new NoRedirectResult();
            }

            await SetLanguage(
                savedUser.User,
                userChatSubscription,
                Enum.Parse<Language>(update.CallbackQuery.Data));

            return new RedirectResult(Route.User);
        }

        private async Task SendRequestMessage(
            Message message, 
            SavedUser savedUser,
            UserChatSubscription chat)
        {
            var markup = new InlineKeyboardMarkup(
                GetLanguageButtons(savedUser, chat));
            
            await Client.EditMessageTextAsync(
                chatId: ContextChat,
                messageId: message.MessageId,
                text: Dictionary.ChooseLanguage,
                replyMarkup: markup);
        }

        private IEnumerable<IEnumerable<InlineKeyboardButton>> GetLanguageButtons(
            SavedUser savedUser,
            UserChatSubscription subscription)
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
                        subscription.Language
                    })
                .Select(LanguageToButton)
                .Batch(2)
                .Concat(
                    new []
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData(
                                Dictionary.Back,
                                $"{Route.User}-{savedUser.Id}"),                             
                        } 
                    });
        }

        private async Task SetLanguage(
            User user,
            UserChatSubscription chat,
            Language language)
        {
            chat.Language = language;
            await _savedUsersRepository.AddOrUpdateAsync(user, chat);
        }
    }
}