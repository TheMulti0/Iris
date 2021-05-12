using System;

namespace ScrapersDistributor
{
    internal class SubscriptionsPollerConfig
    {
        public string ManagerUrl { get; set; }

        public TimeSpan? PollInterval { get; set; } = TimeSpan.FromHours(1);
    }
}