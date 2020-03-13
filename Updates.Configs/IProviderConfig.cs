namespace Updates.Configs
{
    public interface IProviderConfig
    {
        public string[] WatchedUsers { get; set; }
        
        public double PollIntervalSeconds { get; set; }
    }
}