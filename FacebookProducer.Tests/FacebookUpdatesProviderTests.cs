using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacebookProducer.Tests
{
    [TestClass]
    public class FacebookUpdatesProviderTests
    {
        private readonly FacebookUpdatesProvider _updatesProvider;

        public FacebookUpdatesProviderTests()
        {
            _updatesProvider = new FacebookUpdatesProvider();
        }

        [TestMethod]
        public async Task TestGetUpdatesAsync()
        {
            var updates = await _updatesProvider.GetUpdatesAsync("Netanyahu");
            
            Assert.IsNotNull(updates);
            
            CollectionAssert.AllItemsAreNotNull(updates.ToList());
        }
    }
}