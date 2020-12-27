using System;

namespace TelegramReceiver
{
    public class TelegramConfig
    {
        public string AccessToken { get; set; }

        public TimeSpan DefaultInterval { get; set; }
    }
}