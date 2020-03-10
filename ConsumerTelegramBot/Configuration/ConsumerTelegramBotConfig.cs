using System.Text.Json;
using System.Threading.Tasks;

namespace ConsumerTelegramBot.Configuration
{
    internal class ConsumerTelegramBotConfig
    {
        public long[] PostChatIds { get; set; }

        public string Token { get; set; }
        
        public TwitterConfig TwitterConfig { get; set; }
    }
}