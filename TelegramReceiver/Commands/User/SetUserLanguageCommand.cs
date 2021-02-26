using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using SubscriptionsDataLayer;
using Message = Telegram.Bot.Types.Message;
using User = Common.User;

namespace TelegramReceiver
{
    internal class SetUserLanguageCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;
        private readonly Languages _languages;

        public SetUserLanguageCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository,
            Languages languages) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
            _languages = languages;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            CallbackQuery query = Trigger.CallbackQuery;
            var savedUser = await Subscription;
            
            UserChatSubscription userChatSubscription = savedUser.Chats.First(info => info.ChatId == ConnectedChat);
            
            await SendRequestMessage(
                query.Message,
                savedUser,
                userChatSubscription,
                token);

            // Wait for the user to reply with desired language
            
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
            SubscriptionEntity entity,
            UserChatSubscription chat,
            CancellationToken token)
        {
            var markup = new InlineKeyboardMarkup(
                GetLanguageButtons(entity, chat));
            
            await Client.EditMessageTextAsync(
                chatId: ContextChat,
                messageId: message.MessageId,
                text: Dictionary.ChooseLanguage,
                replyMarkup: markup,
                cancellationToken: token);
        }

        private IEnumerable<IEnumerable<InlineKeyboardButton>> GetLanguageButtons(
            SubscriptionEntity entity,
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
                                $"{Route.User}-{entity.Id}"),                             
                        } 
                    });
        }

        private async Task SetLanguage(
            User user,
            UserChatSubscription chat,
            Language language)
        {
            chat.Language = language;
            await _chatSubscriptionsRepository.AddOrUpdateAsync(user, chat);
        }
    }
}