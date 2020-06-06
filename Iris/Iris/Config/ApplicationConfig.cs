using Iris.Api;

namespace Iris.Config
{
    internal class ApplicationConfig
    {
        public TelegramBotConfig TelegramBotConfig { get; set; }
        
        public ProviderConfig TwitterConfig { get; set; }
        

        public ProviderConfig FacebookConfig { get; set; }
    }
}