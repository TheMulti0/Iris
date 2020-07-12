using System.Collections.Generic;

namespace TelegramConsumer
{
    public class TelegramConfig
    {
        public string AccessToken { get; set; }

        public IEnumerable<User> Users { get; set; }
    }
}