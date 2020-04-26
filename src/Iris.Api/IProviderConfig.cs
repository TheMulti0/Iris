namespace Iris.Api
{
    public interface IProviderConfig
    {
        public bool IsEnabled { get; set; }
        
        public User[] WatchedUsers { get; set; }
        
        public double PollIntervalSeconds { get; set; }
    }
}