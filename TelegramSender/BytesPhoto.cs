using Common;

namespace TelegramSender
{
    public record BytesPhoto(byte[] Bytes) : IMedia
    {
        public string Url { get; }
    }
}