using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacebookProducer.Tests
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
            var updates = await _updatesProvider.GetUpdatesAsync("Netanyahu");
            
            Assert.IsNotNull(updates);
            
            CollectionAssert.AllItemsAreNotNull(updates.ToList());
        }
    }
}