using System;

namespace UpdatesProducer
{
    public class PollerConfig
    {
        public TimeSpan Interval { get; set; }
        
        public TimeSpan SendDelay { get; set; }

        public string[] WatchedUserIds { get; set; }

        public bool StoreSentUpdates { get; set; }
    }
}