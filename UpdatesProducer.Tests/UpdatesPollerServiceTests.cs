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
        private static MockUpdatesProducer _producer;
        private static IHostedService _service;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            // TODO add in memory repositories
            var mongoDbConfig = new MongoDbConfig
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "test"
            };

            var pollerConfig = new PollerConfig
            {
                Interval = TimeSpan.Zero,
                WatchedUserIds = new [] { "mockuser" },
                StoreSentUpdates = true
            };

            IServiceCollection addHostedService = new ServiceCollection()
                .AddLogging(builder => builder.AddTestsLogging(context))
                .AddUpdatesProducerMongoRepositories(mongoDbConfig)
                .AddSingleton<IUpdatesProducer, MockUpdatesProducer>()
                .AddSingleton<IUpdatesProvider, MockUpdatesProvider>()
                .AddUpdatesPollerService(pollerConfig);
            
            var services = addHostedService
                .BuildServiceProvider();
            
            _producer = (MockUpdatesProducer) services.GetService<IUpdatesProducer>();
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
            var first = await _producer.Updates.FirstOrDefaultAsync();
            
            Assert.IsNotNull(first);
        }
    }
}