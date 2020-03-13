using Updates.Configs;

namespace Updates.Watcher.Tests
{
    internal class MockProviderConfig : IProviderConfig
    {
        public string[] WatchedUsers { get; set; } = { "0" };

        public double PollIntervalSeconds { get; set; } = 0.1;
    }
}