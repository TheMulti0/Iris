using Common;

namespace TelegramReceiver
{
    public interface IConnectionProperties
    {
        public Language Language { get; set; }

        public long ChatId { get; set; }

        public bool HasAgreedToTos { get; set; }
    }
}