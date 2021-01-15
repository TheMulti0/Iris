using System;
using System.Text.Json.Serialization;

namespace Common
{
    public class UserChatSubscription : IEquatable<UserChatSubscription>
    {
        public TimeSpan Interval { get; set; }

        public string DisplayName { get; set; }

        public Language Language { get; set; } 

        public string ChatId { get; set; }

        public bool ShowPrefix { get; set; } = true;

        public bool ShowSuffix { get; set; }

        public bool SendScreenshotOnly { get; set; }

        public bool Equals(UserChatSubscription other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Interval.Equals(other.Interval) && ChatId == other.ChatId;
        }

        public override int GetHashCode() => HashCode.Combine(Interval, ChatId);

        public static bool operator ==(UserChatSubscription left, UserChatSubscription right) => Equals(left, right);

        public static bool operator !=(UserChatSubscription left, UserChatSubscription right) => !Equals(left, right);
    }
}