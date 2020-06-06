using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Iris.Api;
using Xunit;

namespace Iris.Facebook.Tests
{
    public class FacebookTests
    {
        [Fact]
        public async Task Test()
        {
            var config = JsonExtensions.Read<ProviderConfig>("../../../appsettings.json");

            var facebook = new FacebookProvider(NullLogger<FacebookProvider>.Instance, config);
            IEnumerable<Update> updates = await facebook.GetUpdates(config.WatchedUsers[0]);
            foreach (Update update in updates)
            {
                Assert.NotNull(update?.Id);
                Assert.NotNull(update.Author?.Id);
            }
        }
    }
}