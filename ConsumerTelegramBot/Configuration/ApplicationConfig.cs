using System.Text.Json;
using System.Threading.Tasks;

namespace ConsumerTelegramBot.Configuration
{
    internal class ApplicationConfig
    {
        public TelegramBotConfig TelegramBotConfig { get; set; }
        
        public TwitterConfig TwitterConfig { get; set; }
    }
}