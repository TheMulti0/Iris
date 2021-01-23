using System;

namespace TelegramReceiver
{
    public class TelegramConfig
    {
        public string AccessToken { get; set; }

        public TimeSpan DefaultInterval { get; set; }

        public string[] SuperUsers { get; set; } = new string[0];
    }
}