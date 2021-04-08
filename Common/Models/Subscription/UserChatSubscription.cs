using System;

namespace Common
{
    public record UserChatSubscription
    {
        public TimeSpan Interval { get; set; }

        public ChatInfo ChatInfo { get; set; }

        public bool SendScreenshotOnly { get; set; }

        public Text Prefix { get; set; }

        public Text Suffix { get; set; }

        public bool ShowUrlPreview { get; set; }
        
        public DateTime SubscriptionDate { get; set; }
    }
}