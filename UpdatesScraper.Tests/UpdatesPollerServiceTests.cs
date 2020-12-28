using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdatesScraper.Tests
{
    [TestClass]
    public class UpdatesPollerServiceTests
    {
        private static UpdatesScraper _scraper;
        private static IHostedService _service;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var pollerConfig = new ScraperConfig
            {
                StoreSentUpdates = true
            };

            IServiceCollection addHostedService = new ServiceCollection()
                .AddLogging(builder => builder.AddTestsLogging(context))
                .AddUpdatesScraperMockRepositories()
                .AddSingleton<IProducer<Update>, MockUpdatesProducer>()
                .AddSingleton<IUpdatesProvider, MockUpdatesProvider>()
                .AddVideoExtractor(new VideoExtractorConfig())
                .AddUpdatesScraper(pollerConfig);
            
            var services = addHostedService
                .BuildServiceProvider();
            
            _scraper = services.GetRequiredService<UpdatesScraper>();
            _service = services.GetService<IHostedService>();
        }
        
        [TestInitialize]
        public void Start()
        {
            Task.Factory
                .StartNew(async () => await _service.StartAsync(CancellationToken.None));
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task TestScrape()
        {
            var first = await _scraper.ScrapeUser(new User("test", Platform.Facebook), CancellationToken.None)
                .FirstOrDefaultAsync();
            
            Assert.IsNotNull(first);
        }
    }
}