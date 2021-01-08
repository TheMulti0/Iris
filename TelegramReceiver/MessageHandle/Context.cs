using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;
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

        public Task<Update> NextMessageTask { get; } =
            IncomingUpdates
                .FirstAsync(update => update.Type == UpdateType.Message)
                .ToTask();
    }
}