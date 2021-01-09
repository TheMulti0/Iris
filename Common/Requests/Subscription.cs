using System;
using System.Text.Json.Serialization;

namespace Common
{
    public record Subscription
    {
        public User User { get; }
        
        [JsonConverter(typeof(NullableTimeSpanConverter))]
        public TimeSpan? Interval { get; }

        public Subscription(User user, TimeSpan? interval)
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