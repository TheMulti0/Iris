using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Updates.Api;
using Updates.Configs;
using Xunit;

namespace Updates.Twitter.Tests
{
    public class TwitterTests
    {
        [Fact]
        public async Task Test()
        {
            var config = JsonSerializer.Deserialize<TwitterConfig>(
                File.ReadAllText("../../../appsettings.json"));
            
            var twitter = new Twitter(NullLogger<Twitter>.Instance, config);
            IEnumerable<Update> updates = await twitter.GetUpdates(25073877); // @realDonaldTrump
            foreach (Update update in updates)
            {
                Assert.NotNull(update?.Id);
                Assert.NotNull(update.Author?.Id);
            }
        }
    }
}