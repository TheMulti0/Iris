using Common;

namespace TelegramReceiver
{
    public interface IConnectionProperties
    {
        public Language Language { get; set; }

        public string Chat { get; set; }

        public bool HasAgreedToTos { get; set; }
    }
}