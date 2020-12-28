using System;

namespace Common
{
    public record User(
        string UserId,
        Platform Platform)
    {
        public virtual bool Equals(User? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return UserId == other.UserId && Platform == other.Platform;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, Platform);
        }

        public override string ToString() => $"{UserId} ({Platform})";
    }
}