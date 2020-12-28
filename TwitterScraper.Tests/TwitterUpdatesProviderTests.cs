using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common;
using NUnit.Framework;

namespace TwitterScraper.Tests
{
    public class TwitterUpdatesProviderTests
    {
        private readonly TwitterUpdatesProvider _twitter;

        public TwitterUpdatesProviderTests()
        {
            _twitter = new TwitterUpdatesProvider(
                JsonSerializer.Deserialize<TwitterUpdatesProviderConfig>(
                    File.ReadAllText("appsettings.json")));
        }

        [Test]
        public async Task Test1()
        {
            List<Update> updates = (await _twitter.GetUpdatesAsync(new User("@realDonaldTrump", Platform.Twitter))).ToList();
            
            Assert.IsNotNull(updates);
            CollectionAssert.AllItemsAreNotNull(updates);
        }
    }
}