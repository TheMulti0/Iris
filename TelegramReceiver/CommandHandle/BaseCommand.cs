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
        protected readonly Task<Update> NextUpdate;
        protected readonly Update Trigger;
        protected readonly ChatId ContextChat;
        protected readonly ChatId ConnectedChat;
        protected readonly Language Language;
        protected readonly LanguageDictionary Dictionary;
        protected readonly User SelectedUser;
        
        protected BaseCommand(Context context)
        {
            Context = context;
            
            (Client, NextUpdate, Trigger, ContextChat, ConnectedChat, Language, Dictionary) = context;

            SelectedUser = context.SelectedUser;
        }
        
        
    }
}