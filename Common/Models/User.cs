using System;

namespace Common
{
    public record User(
        string UserId,
        string Source)
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
            return UserId == other.UserId && Source == other.Source;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, Source);
        }

        public void Deconstruct(out string userId, out string source)
        {
            userId = UserId;
            source = Source;
        }

        public override string ToString() => $"{UserId} ({Source})";
    }
}