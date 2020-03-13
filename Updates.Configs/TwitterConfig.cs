namespace Updates.Configs
{
    public class TwitterConfig : IProviderConfig
    {
        public string[] WatchedUsers { get; set; }
        
        public double PollIntervalSeconds { get; set; }

        public int MaxResults { get; set; } = 40;

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }
    }
}