using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Nito.AsyncEx;
using Telegram.Bot;
using Telegram.Bot.Types;
using SubscriptionsDb;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    public abstract class BaseCommand
    {
        protected readonly Context Context;
        protected readonly ITelegramBotClient Client;
        protected readonly AsyncLazy<SubscriptionEntity> Subscription;
        protected readonly Func<Task<Update>> GetNextMessage;
        protected readonly Func<Task<Update>> GetNextCallbackQuery;
        protected readonly Update Trigger;
        protected readonly ChatId ContextChat;
        protected readonly LanguageDictionary Dictionary;
        protected readonly Connection Connection;
        protected readonly bool IsSuperUser;
        protected readonly Platform? SelectedPlatform;
        protected readonly ChatId ConnectedChat;
        protected readonly Language Language;
        
        protected BaseCommand(Context context)
        {
            Context = context;
            
            (Client, Subscription, GetNextMessage, GetNextCallbackQuery, Trigger, ContextChat, Connection, Dictionary, IsSuperUser) = context;

            SelectedPlatform = context.SelectedPlatform 
                               ?? ExtractPlatform(Trigger?.CallbackQuery) 
                               ?? Subscription.Task.Result?.User?.Platform;
            
            ConnectedChat = Connection?.Chat ?? ContextChat;
            Language = Connection?.Language ?? Language.English;
        }

        private static Platform? ExtractPlatform(CallbackQuery query)
        {
            try
            {
                string[] items = query.Data.Split("-");
            
                return Enum.Parse<Platform>(items.Last());
            }
            catch
            {
                return null;
            }
        }

        protected TextType GetTextType()
        {
            return Enum.Parse<TextType>(Trigger.CallbackQuery.Data.Split("-")[1]); 
        }

        protected string GetChatTitle(Chat connectedChat)
        {
            return string.IsNullOrEmpty(connectedChat?.Title) 
                ? Dictionary.PrivateDm 
                : $"{connectedChat.Title}\n({connectedChat.Id})";
        } 
    }
}