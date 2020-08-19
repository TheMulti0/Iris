using Telegram.Bot.Types.Enums;

namespace TelegramConsumer
{
    public static class TelegramConstants
    {
        public const int MaxTextMessageLength = 4096;
        
        public const int MaxMediaCaptionLength = 1024;
        
        public const ParseMode MessageParseMode = ParseMode.Html;
        
        public const bool DisableWebPagePreview = true;
    }
}