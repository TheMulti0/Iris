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

            var savedUser = await _savedUsersRepository.GetAsync(SelectedUser);
            UserChatSubscription userChatSubscription = savedUser.Chats.First(info => info.ChatId == ConnectedChat);
            
            await SendRequestMessage(
                query.Message,
                userChatSubscription);

            // Wait for the user to reply with desired display name
            
            var update = await NextCallbackQuery;

            if (update.CallbackQuery.Data.StartsWith(Route.User.ToString()))
            {
                return new NoRedirectResult();
            }

            await SetLanguage(
                userChatSubscription,
                Enum.Parse<Language>(update.CallbackQuery.Data));

            return new RedirectResult(Route.User);
        }

        private async Task SetLanguage(
            UserChatSubscription chat,
            Language language)
        {
            chat.Language = language;
            await _savedUsersRepository.AddOrUpdateAsync(SelectedUser, chat);
        }
        
        private IEnumerable<IEnumerable<InlineKeyboardButton>> GetLanguageButtons(UserChatSubscription subscription)
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
                                $"{Route.User}-{SelectedUser.UserId}-{Enum.GetName(SelectedUser.Platform)}"),                             
                        } 
                    });
        }

        private async Task SendRequestMessage(
            Message message, UserChatSubscription chat)
        {
            var markup = new InlineKeyboardMarkup(
                GetLanguageButtons(chat));
            
            await Client.EditMessageTextAsync(
                chatId: ContextChat,
                messageId: message.MessageId,
                text: Dictionary.ChooseLanguage,
                replyMarkup: markup);
        }
    }
}