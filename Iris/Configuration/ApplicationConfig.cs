using System.Text.Json;
using System.Threading.Tasks;

namespace Iris.Configuration
{
    internal class ApplicationConfig
    {
        public TelegramBotConfig TelegramBotConfig { get; set; }
        
        public TwitterConfig TwitterConfig { get; set; }
    }
}