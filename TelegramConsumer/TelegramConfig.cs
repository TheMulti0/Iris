using System.Collections.Generic;
using Telegram.Bot.Types;

namespace TelegramConsumer
{
    public class TelegramConfig
    {
        public string AccessToken { get; set; }

        public Dictionary<string, ChatId[]> UsernamesChatIds { get; set; }
    }
}