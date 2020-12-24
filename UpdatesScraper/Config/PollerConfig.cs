using System;

namespace UpdatesScraper
{
    public class PollerConfig
    {
        public TimeSpan Interval { get; set; }
        
        // TODO find a solution to ordering inside RabbitMQ that will eventually lead to removing this property
        public TimeSpan SendDelay { get; set; }

        public string[] WatchedUserIds { get; set; }

        public bool StoreSentUpdates { get; set; }
    }
}