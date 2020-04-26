using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Updates.Api;
using Xunit;

namespace Updates.Facebook.Tests
{
    public class FacebookTests
    {
        [Fact]
        public async Task Test()
        {
            var config = JsonExtensions.Read<FacebookConfig>("../../../appsettings.json");

            var facebook = new Facebook(NullLogger<Facebook>.Instance, config);
            IEnumerable<Update> updates = await facebook.GetUpdates(config.WatchedUsers[0]);
            foreach (Update update in updates)
            {
                Assert.NotNull(update?.Id);
                Assert.NotNull(update.Author?.Id);
            }
        }
    }
}