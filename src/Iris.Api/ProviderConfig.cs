namespace Iris.Api
{
    public class ProviderConfig
    {
        public bool IsEnabled { get; set; }
        
        public User[] WatchedUsers { get; set; }
        
        public int PageCountPerUser { get; set; }
        
        public double PollIntervalSeconds { get; set; }

        public string ScraperUrl { get; set; }
    }
}