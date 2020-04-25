namespace Updates.Configs
{
    public class FacebookConfig : IProviderConfig
    {
        public bool IsEnabled { get; set; }
        
        public string[] WatchedUsers { get; set; }
        
        public int PageCountPerUser { get; set; }
        
        public double PollIntervalSeconds { get; set; }

        public string ScraperUrl { get; set; }
    }
}