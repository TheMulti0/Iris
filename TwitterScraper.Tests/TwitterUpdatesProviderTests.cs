using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TwitterScraper.Tests
{
    [TestClass]
    public class TwitterUpdatesProviderTests
    {
        private readonly TwitterUpdatesProvider _twitter;

        public TwitterUpdatesProviderTests()
        {
            var rootConfig = new ConfigurationBuilder().AddUserSecrets<TwitterUpdatesProviderConfig>().Build();

            var config = rootConfig.GetSection<TwitterUpdatesProviderConfig>("UpdatesProvider");
            
            _twitter = new TwitterUpdatesProvider(config);
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