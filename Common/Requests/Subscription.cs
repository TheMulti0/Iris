using System;
using System.Text.Json.Serialization;

namespace Common
{
    public record Subscription
    {
        public User User { get; }
        
        [JsonConverter(typeof(NullableTimeSpanConverter))]
        public TimeSpan? Interval { get; }

        public DateTime? MinimumEarliestUpdateTime { get; set; }

        public Subscription(User user, TimeSpan? interval)
        {
            User = user;
            Interval = interval;
        }
        
        [JsonConstructor]
        public Subscription(User user, TimeSpan? interval, DateTime? minimumEarliestUpdateTime)
        {
            User = user;
            Interval = interval;
            MinimumEarliestUpdateTime = minimumEarliestUpdateTime;
        }

        public void Deconstruct(out User user, out TimeSpan? interval)
        {
            user = User;
            interval = Interval;
        }
    }
}