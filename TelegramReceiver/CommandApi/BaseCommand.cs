using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    public abstract class BaseCommand
    {
        protected readonly Context Context;
        protected readonly ITelegramBotClient Client;
        protected readonly Task<Update> NextMessage;
        protected readonly Task<Update> NextCallbackQuery;
        protected readonly Update Trigger;
        protected readonly ChatId ContextChat;
        protected readonly ChatId ConnectedChat;
        protected readonly Language Language;
        protected readonly LanguageDictionary Dictionary;
        protected readonly User SelectedUser;
        protected readonly Platform? SelectedPlatform;
        
        protected BaseCommand(Context context)
        {
            Context = context;
            
            (Client, NextMessage, NextCallbackQuery, Trigger, ContextChat, ConnectedChat, Language, Dictionary) = context;

            SelectedUser = context.SelectedUser;

            SelectedPlatform = SelectedUser?.Platform ?? ExtractPlatform(Trigger?.CallbackQuery);
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
    }
}