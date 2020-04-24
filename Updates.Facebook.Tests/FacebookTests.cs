using System.Collections.Generic;
using System.Threading.Tasks;
using Updates.Api;
using Updates.Configs;
using Xunit;

namespace Updates.Facebook.Tests
{
    public class FacebookTests
    {
        [Fact]
        public async Task Test()
        {
            var config = JsonExtensions.Read<FacebookConfig>("../../../appsettings.json");

            var facebook = new Facebook(config);
            IEnumerable<Update> updates = await facebook.GetUpdates(config.WatchedUsers[0]);
            foreach (Update update in updates)
            {
                Assert.NotNull(update?.Id);
                Assert.NotNull(update.Author?.Id);
            }
        }
    }
}