using System;
using System.Threading.Tasks;
using Common;
using Nito.AsyncEx;
using Telegram.Bot;
using Telegram.Bot.Types;
using SubscriptionsDataLayer;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    public record Context(
        ITelegramBotClient Client,
        AsyncLazy<SubscriptionEntity> SavedUser, 
        Func<Task<Update>> GetNextMessage,
        Func<Task<Update>> GetNextCallbackQuery,
        Update Trigger,
        ChatId ContextChatId,
        Connection Connection,
        LanguageDictionary Dictionary,
        bool IsSuperUser)
    {
        public Platform? SelectedPlatform { get; init; }

        public Chat ConnectedChat { get; init; }
    }
}