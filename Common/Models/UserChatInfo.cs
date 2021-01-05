using System;
using System.Text.Json.Serialization;

namespace Common
{
    public class UserChatInfo : IEquatable<UserChatInfo>
    {
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Interval { get; set; }

        public string DisplayName { get; set; }

        public Language Language { get; set; } 

        public string ChatId { get; set; }

        public bool ShowPrefix { get; set; } = true;

        public bool Equals(UserChatInfo other)
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

        public static bool operator ==(UserChatInfo left, UserChatInfo right) => Equals(left, right);

        public static bool operator !=(UserChatInfo left, UserChatInfo right) => !Equals(left, right);
    }
}