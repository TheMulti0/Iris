using System;

namespace UserDataLayer
{
    public class ChatInfo : IEquatable<ChatInfo>
    {
        public TimeSpan Interval { get; set; }

        public string Chat { get; set; }

        public bool Equals(ChatInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Interval.Equals(other.Interval) && Chat == other.Chat;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((ChatInfo) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Interval, Chat);

        public static bool operator ==(ChatInfo left, ChatInfo right) => Equals(left, right);

        public static bool operator !=(ChatInfo left, ChatInfo right) => !Equals(left, right);
    }
}