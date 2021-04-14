using System;

namespace TelegramClient
{
    public class TelegramClientConfig
    {
        public int AppId { get; set; }

        public string AppHash { get; set; }

        public string BotToken { get; set; }

        public TimeSpan MessageSendTimeout { get; set; } = TimeSpan.FromHours(1);
    }
}