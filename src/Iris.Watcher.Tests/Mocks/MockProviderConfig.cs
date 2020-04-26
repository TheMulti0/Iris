using Iris.Api;

namespace Iris.Watcher.Tests
{
    internal class MockProviderConfig : IProviderConfig
    {
        public bool IsEnabled { get; set; } = true;
        
        public User[] WatchedUsers { get; set; } = { new User() };

        public double PollIntervalSeconds { get; set; } = 0.1;
    }
}