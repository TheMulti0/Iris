using System.Collections.Generic;

namespace TelegramBot
{
    public class TelegramConfig
    {
        public string AccessToken { get; set; }

        public IEnumerable<User> Users { get; set; }
    }
}