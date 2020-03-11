using Updates.Configs;

namespace Iris.Config
{
    internal class ApplicationConfig
    {
        public TelegramBotConfig TelegramBotConfig { get; set; }
        
        public TwitterConfig TwitterConfig { get; set; }
    }
}