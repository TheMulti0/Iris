using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdatesProducer.Tests
{
    [TestClass]
    public class UpdatesPollerServiceTests
    {
        private static MockUpdatesPublisher _publisher;
        private static IHostedService _service;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var pollerConfig = new PollerConfig
            {
                Interval = TimeSpan.Zero,
                WatchedUserIds = new [] { "mockuser" },
                StoreSentUpdates = true
            };

            IServiceCollection addHostedService = new ServiceCollection()
                .AddLogging(builder => builder.AddTestsLogging(context))
                .AddUpdatesProducerMockRepositories()
                .AddSingleton<IUpdatesPublisher, MockUpdatesPublisher>()
                .AddSingleton<IUpdatesProvider, MockUpdatesProvider>()
                .AddUpdatesPollerService(pollerConfig);
            
            var services = addHostedService
                .BuildServiceProvider();
            
            _publisher = (MockUpdatesPublisher) services.GetService<IUpdatesPublisher>();
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
        public async Task TestProduce()
        {
            var first = await _publisher.Updates.FirstOrDefaultAsync();
            
            Assert.IsNotNull(first);
        }
    }
}