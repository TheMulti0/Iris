using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdatesScraper;

namespace FeedsScraper.Tests
{
    [TestClass]
    public class FeedUpdatesProviderTests
    {
        private readonly FeedUpdatesProvider _provider;

        public FeedUpdatesProviderTests()
        {
            _provider = new FeedUpdatesProvider();
        }

        [TestMethod]
        public async Task Test1()
        {
            List<Update> updates = (await _provider.GetUpdatesAsync(new User("http://feeds.soundcloud.com/users/soundcloud:users:108885014/sounds.rss", Platform.Feeds))).ToList();
            
            Assert.IsNotNull(updates);
            CollectionAssert.AllItemsAreNotNull(updates);
        }
    }
}