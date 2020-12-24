using System;

namespace IrisPoc
{
    internal record UserPollRule(
        User User,
        TimeSpan? Interval)
    {
        public virtual bool Equals(UserPollRule other)
        {
            return other?.User == User;
        }

        public override int GetHashCode()
        {
            return User?.GetHashCode() ?? 0;
        }
    }
}