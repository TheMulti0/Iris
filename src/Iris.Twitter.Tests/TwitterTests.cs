using System.Collections.Generic;
using System.Threading.Tasks;
using Iris.Api;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Iris.Twitter.Tests
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