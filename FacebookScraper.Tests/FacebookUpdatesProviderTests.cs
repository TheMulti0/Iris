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
            List<Update> updates = (await _updatesProvider.GetUpdatesAsync(new User("Netanyahu", Platform.Facebook)))?.ToList();
            
            Assert.IsNotNull(updates);

            CollectionAssert.AllItemsAreNotNull(updates);
        }
    }
}