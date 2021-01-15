using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TwitterScraper.Tests
{
    [TestClass]
    public class TwitterUpdatesProviderTests
    {
        private readonly TwitterUpdatesProvider _twitter;

        public TwitterUpdatesProviderTests()
        {
            _twitter = new TwitterUpdatesProvider(
                JsonSerializer.Deserialize<TwitterUpdatesProviderConfig>(
                    File.ReadAllText("appsettings.json")));
        }

        [TestMethod]
        public async Task Test1()
        {
            List<Update> updates = (await _twitter.GetUpdatesAsync(new User("themulti0", Platform.Twitter))).ToList();
            
            Assert.IsNotNull(updates);
            CollectionAssert.AllItemsAreNotNull(updates);
        }
    }
}