using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdatesScraper;

namespace FacebookScraper.Tests
{
    [TestClass]
    public class FacebookUpdatesProviderTests
    {
        private static FacebookUpdatesProvider _updatesProvider;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var loggerFactory = new LoggerFactory(
                new[]
                {
                    new TestsLoggerProvider(context)
                });
            
            _updatesProvider = new FacebookUpdatesProvider(
                loggerFactory.CreateLogger<FacebookUpdatesProvider>());
        }

        [TestMethod]
        public async Task TestGetUpdatesAsync()
        {
            List<Update> updates = (await _updatesProvider.GetUpdatesAsync(new User("%D7%A8%D7%95%D7%A0%D7%99-%D7%A1%D7%A1%D7%95%D7%91%D7%A8-Roni-Sassover-100178875444889", Platform.Facebook)))?.ToList();
            
            Assert.IsNotNull(updates);

            CollectionAssert.AllItemsAreNotNull(updates);
        }

        [TestMethod]
        public async Task TestNoUpdatesAsync()
        {
            List<Update> updates = (await _updatesProvider.GetUpdatesAsync(new User("thisactuallyisaninvalidfacebookuseraccountforthesakeoftesting", Platform.Facebook)))?.ToList();
            
            Assert.IsNotNull(updates);

            CollectionAssert.AllItemsAreNotNull(updates);
        }
    }
}