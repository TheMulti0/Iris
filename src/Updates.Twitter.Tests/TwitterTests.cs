using System.Collections.Generic;
using System.Threading.Tasks;
using Iris.Api;
using Iris.Twitter;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Updates.Twitter.Tests
{
    public class TwitterTests
    {
        [Fact]
        public async Task Test()
        {
            var config = JsonExtensions.Read<TwitterConfig>("../../../appsettings.json");
            
            var twitter = new Iris.Twitter.Twitter(NullLogger<Iris.Twitter.Twitter>.Instance, config);
            IEnumerable<Update> updates = await twitter.GetUpdates("@realDonaldTrump");
            foreach (Update update in updates)
            {
                Assert.NotNull(update?.Id);
                Assert.NotNull(update.Author?.Id);
            }
        }
    }
}