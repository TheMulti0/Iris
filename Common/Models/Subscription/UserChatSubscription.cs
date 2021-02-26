using System;
using System.Text.Json.Serialization;

namespace Common
{
    public record UserChatSubscription
    {
        public TimeSpan Interval { get; set; }

        public string DisplayName { get; set; }

        public Language Language { get; set; } 

        public string ChatId { get; set; }

        public bool SendScreenshotOnly { get; set; }

        public bool ShowPrefix { get; set; }
        
        public bool ShowSuffix { get; set; }

        public Text Prefix { get; set; }

        public Text Suffix { get; set; }

        public bool ShowUrlPreview { get; set; }
    }
}