namespace Updates.Configs
{
    public interface IProviderConfig
    {
        public bool IsEnabled { get; set; }
        
        public string[] WatchedUsers { get; set; }
        
        public double PollIntervalSeconds { get; set; }
    }
}