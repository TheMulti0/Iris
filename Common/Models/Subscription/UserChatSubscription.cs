using System;

namespace Common
{
    public record UserChatSubscription
    {
        public TimeSpan Interval { get; set; }

        public string ChatId { get; set; }

        public bool SendScreenshotOnly { get; set; }

        public Text Prefix { get; set; }

        public Text Suffix { get; set; }

        public bool ShowUrlPreview { get; set; }
        
        public DateTime SubscriptionDate { get; set; }
    }
}