using System.Linq;
using System.Reactive.Linq;
using Iris.Api;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Iris.Watcher.Tests
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
                var watchedUser = config.WatchedUsers.ElementAtOrDefault(i) ?? config.WatchedUsers.LastOrDefault();
                var author = updates[i].Author;
                
                Assert.Equal(
                    watchedUser,
                    author);
            }
        }
    }
}