using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Updates.Api;
using Xunit;

namespace Updates.Twitter.Tests
{
    public class TwitterTests
    {
        [Fact]
        public async Task Test()
        {
            var config = JsonExtensions.Read<TwitterConfig>("../../../appsettings.json");
            
            var twitter = new Twitter(NullLogger<Twitter>.Instance, config);
            IEnumerable<Update> updates = await twitter.GetUpdates("@realDonaldTrump");
            foreach (Update update in updates)
            {
                Assert.NotNull(update?.Id);
                Assert.NotNull(update.Author?.Id);
            }
        }
    }
}