using Updates.Api;

namespace Updates.Watcher.Tests
{
    internal class MockProviderConfig : IProviderConfig
    {
        public bool IsEnabled { get; set; } = true;
        
        public string[] WatchedUsers { get; set; } = { "0" };

        public double PollIntervalSeconds { get; set; } = 0.1;
    }
}