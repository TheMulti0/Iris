using System;
using System.Text.Json.Serialization;

namespace Common
{
    public record UserPollRule
    {
        public User User { get; }
        
        public TimeSpan? Interval { get; }

        public UserPollRule(User user, TimeSpan? interval)
        {
            User = user;
            Interval = interval;
        }

        public void Deconstruct(out User user, out TimeSpan? interval)
        {
            user = User;
            interval = Interval;
        }
    }
}