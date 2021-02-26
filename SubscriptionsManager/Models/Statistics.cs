using System;

namespace SubscriptionsManager
{
    public record Statistics(
        int SubscriptionsCount,
        int ChatsCount)
    {
        public DateTime ReportTime { get; init; } = DateTime.Now;
    }
}