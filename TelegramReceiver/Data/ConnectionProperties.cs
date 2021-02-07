using Common;

namespace TelegramReceiver
{
    public class ConnectionProperties : IConnectionProperties
    {
        public Language Language { get; set; }
        public string Chat { get; set; }
        public bool HasAgreedToTos { get; set; }
    }
}