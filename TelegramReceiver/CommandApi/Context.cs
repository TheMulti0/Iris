using System;
using System.Threading.Tasks;
using Common;
using Nito.AsyncEx;
using Telegram.Bot;
using Telegram.Bot.Types;
using SubscriptionsDb;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    public record Context(
        ITelegramBotClient Client,
        AsyncLazy<SubscriptionEntity> Subscription, 
        Func<Task<Update>> GetNextMessage,
        Func<Task<Update>> GetNextCallbackQuery,
        Update Trigger,
        long ContextChatId,
        Connection Connection,
        LanguageDictionary Dictionary,
        bool IsSuperUser)
    {
        public Platform? SelectedPlatform { get; init; }

        public Chat ConnectedChat { get; init; }
    }
}