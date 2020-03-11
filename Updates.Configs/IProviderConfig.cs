namespace Updates.Configs
{
    public interface IProviderConfig
    {
        public long[] WatchedUsersIds { get; set; }
        
        public double PollIntervalSeconds { get; set; }
    }
}