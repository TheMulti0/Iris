using Iris.Api;

namespace Iris.Twitter
{
    public class TwitterConfig : IProviderConfig
    {
        public bool IsEnabled { get; set; }
        
        public User[] WatchedUsers { get; set; }
        
        public int PageCountPerUser { get; set; }
        
        public double PollIntervalSeconds { get; set; }

        public string ScraperUrl { get; set; }
    }
}