using Updates.Configs;

namespace Updates.Watcher.Tests
{
    internal class MockProviderConfig : IProviderConfig
    {
        public long[] WatchedUsersIds { get; set; } = { 0L };

        public double PollIntervalSeconds { get; set; } = 0.1;
    }
}