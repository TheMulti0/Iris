﻿using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramReceiver
{
    public static class UpdateExtensions
    {
        public static long GetChatId(this Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    return update.Message.Chat.Id;
                case UpdateType.CallbackQuery:
                    return update.CallbackQuery.Message.Chat.Id;
                default:
                    return 0;
            }
        }
        
        public static int GetMessageId(this Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    return update.Message.MessageId;
                case UpdateType.CallbackQuery:
                    return update.CallbackQuery.Message.MessageId;
                default:
                    return 1;
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