namespace Iris.Api
{
    public interface IProviderConfig
    {
        bool IsEnabled { get; set; }
        
        User[] WatchedUsers { get; set; }
        
        int PageCountPerUser { get; set; }
        
        double PollIntervalSeconds { get; set; }

        string ScraperUrl { get; set; }
    }
}