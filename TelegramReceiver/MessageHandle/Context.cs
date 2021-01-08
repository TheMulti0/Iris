using System;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    public record Context(
        ITelegramBotClient Client,
        IObservable<Update> IncomingUpdates,
        Update Update,
        ChatId ContextChatId,
        ChatId ConnectedChatId,
        Language Language,
        LanguageDictionary LanguageDictionary)
    {
        public User SelectedSavedUser { get; init; }
    }
}