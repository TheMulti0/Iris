using System;
using System.Text.Json.Serialization;

namespace Common
{
    public record Subscription
    {
        public string UserId { get; }
        
        public string Platform { get; }
        
        [JsonConverter(typeof(NullableTimeSpanConverter))]
        public TimeSpan? Interval { get; init; }

        public DateTime? MinimumEarliestUpdateTime { get; set; }

        public Subscription(string userId, string platform, TimeSpan? interval)
        {
            UserId = userId;
            Platform = platform;
            Interval = interval;
        }
        
        [JsonConstructor]
        public Subscription(string userId, string platform, TimeSpan? interval, DateTime? minimumEarliestUpdateTime)
        {
            UserId = userId;
            Platform = platform;
            Interval = interval;
            MinimumEarliestUpdateTime = minimumEarliestUpdateTime;
        }

        public override string ToString()
        {
            return MinimumEarliestUpdateTime == null
                ? $"{{ Subscription: [{Platform}] {UserId}, Interval: {Interval} }}"
                : $"{{ Subscription: [{Platform}] {UserId}, Interval: {Interval}, From: {MinimumEarliestUpdateTime} }}";
        }

        public void Deconstruct(out string userId, out string platform, out TimeSpan? interval)
        {
            userId = UserId;
            platform = Platform;
            interval = Interval;
        }
    }
}