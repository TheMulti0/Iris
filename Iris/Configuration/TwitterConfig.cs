using System;

namespace Iris.Configuration
{
    internal class TwitterConfig
    {
        public long[] WatchedUsersIds { get; set; }
        
        public double PollIntervalSeconds { get; set; }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }
    }
}