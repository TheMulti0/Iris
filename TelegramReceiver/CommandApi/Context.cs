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
        Task<Update> NextMessage,
        Task<Update> NextCallbackQuery,
        Update Trigger,
        ChatId ContextChatId,
        ChatId ConnectedChatId,
        Language Language,
        LanguageDictionary LanguageDictionary)
    {
        public User SelectedUser { get; init; } = ExtractUser(Trigger?.CallbackQuery);
        
        public Platform? SelectedPlatform { get; init; }

        public Chat ConnectedChat { get; init; }
        
        private static User ExtractUser(CallbackQuery query)
        {
            try
            {
                string[] items = query.Data.Split("-");
            
                return new User(items[^2], Enum.Parse<Platform>(items[^1]));
            }
            catch
            {
                return null;
            }
        }
    }
}