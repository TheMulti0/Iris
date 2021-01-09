using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramReceiver.Data;

namespace TelegramReceiver
{
    internal class SetLanguageCommand : ICommand
    {
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly Languages _languages;
        private readonly UsersCommand _command;

        public const string CallbackPath = "setLanguage";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public SetLanguageCommand(
            IConnectionsRepository connectionsRepository,
            Languages languages,
            UsersCommand command)
        {
            _languages = languages;
            _command = command;
            _connectionsRepository = connectionsRepository;
        }

        public async Task OperateAsync(Context context)
        {
            CallbackQuery query = context.Trigger.CallbackQuery;

            Language language = GetLanguage(query);

            await _connectionsRepository.AddOrUpdateAsync(
                    context.Trigger.GetUser(),
                    context.ConnectedChatId,
                    language);
            
            LanguageDictionary newLanguageDictionary = _languages.Dictionary[language];
            
            await _command.OperateAsync(
                context with { Language = language, LanguageDictionary = newLanguageDictionary });
        }

        private static Language GetLanguage(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return Enum.Parse<Language>(items.Last());
        }
    }
}