using Iris.Api;

namespace Iris.Watcher.Tests
{
    internal class MockProviderConfig : ProviderConfig
    {
        public bool IsEnabled { get; set; } = true;
        
        public User[] WatchedUsers { get; set; } = { new User() };

        public int PageCountPerUser { get; set; } = 1;

        public double PollIntervalSeconds { get; set; } = 0.1;

        public string ScraperUrl { get; set; } = "";
    }
}