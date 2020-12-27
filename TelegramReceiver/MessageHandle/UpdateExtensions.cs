using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramReceiver
{
    public static class UpdateExtensions
    {
        public static ChatId GetChatId(this Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    return update.Message.Chat.Id;
                case UpdateType.CallbackQuery:
                    return update.CallbackQuery.Message.Chat.Id;
                default:
                    return null;
            }
        }
        
        public static User GetUser(this Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    return update.Message.From;
                case UpdateType.CallbackQuery:
                    return update.CallbackQuery.From;
                default:
                    return null;
            }
        }
    }
}