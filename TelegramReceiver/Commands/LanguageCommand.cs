using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    internal class LanguageCommand : BaseCommand, ICommand
    {
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly Languages _languages;

        public LanguageCommand(
            Context context,
            IConnectionsRepository connectionsRepository,
            Languages languages) : base(context)
        {
            _connectionsRepository = connectionsRepository;
            _languages = languages;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            await SendLanguageList(token);

            Update update = await NextCallbackQuery;
            
            if (update == null || update.CallbackQuery.Data.StartsWith(Route.Settings.ToString()))
            {
                return new NoRedirectResult();
            }

            var language = Enum.Parse<Language>(update.CallbackQuery.Data);

            await _connectionsRepository.AddOrUpdateAsync(
                update.GetUser(),
                ConnectedChat,
                language);
            
            LanguageDictionary newLanguageDictionary = _languages.Dictionary[language];

            return new RedirectResult(
                Route.Settings,
                Context with { Language = language, LanguageDictionary = newLanguageDictionary });
        }

        private async Task SendLanguageList(CancellationToken token)
        {
            string text = Dictionary.ChooseLanguage;
            var markup = new InlineKeyboardMarkup(GetLanguageButtons());

            if (Trigger.Type == UpdateType.CallbackQuery)
            {
                await Client.EditMessageTextAsync(
                    chatId: ContextChat,
                    messageId: Trigger.GetMessageId(),
                    text: text,
                    replyMarkup: markup,
                    cancellationToken: token);
            }
            else
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: text,
                    replyMarkup: markup,
                    cancellationToken: token);
            }
        }

        private IEnumerable<IEnumerable<InlineKeyboardButton>> GetLanguageButtons()
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
                        Language
                    })
                .Select(LanguageToButton)
                .Concat(
                    new []
                    { 
                        InlineKeyboardButton
                            .WithCallbackData(Dictionary.Back, Route.Settings.ToString())
                    })
                .Batch(1);
        }
    }
}