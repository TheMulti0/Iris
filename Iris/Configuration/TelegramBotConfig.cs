namespace Iris.Configuration
{
    internal class TelegramBotConfig
    {
        public string Token { get; set; }

        public long[] UpdateChatsIds { get; set; }
    }
}