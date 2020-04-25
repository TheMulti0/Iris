using System.Linq;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Updates.Api;
using Xunit;

namespace Updates.Watcher.Tests
{
    public class UpdatesWatcherTests
    {
        [Fact]
        public void Test()
        {
            var config = new MockProviderConfig();

            var watcher = new UpdatesWatcher(
                NullLogger<UpdatesWatcher>.Instance,
                new MockUpdatesProvider(),
                config);

            Update[] updates = watcher.Updates.Take(10).ToEnumerable().ToArray();
            
            for (var i = 0; i < updates.Length; i++)
            {
                string watchedUser = config.WatchedUsers.ElementAtOrDefault(i) ?? config.WatchedUsers.LastOrDefault();
                string author = updates[i].Author.Name;
                
                Assert.Equal(
                    watchedUser,
                    author);
            }
        }
    }
}