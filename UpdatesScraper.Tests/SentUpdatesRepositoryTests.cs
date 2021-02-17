using System;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdatesScraper.Tests
{
    [TestClass]
    public class SentUpdatesRepositoryTests
    {
        private static ISentUpdatesRepository _repository;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddTestsLogging(context))
                .AddMongoDb(
                    new MongoDbConfig
                    {
                        ConnectionString = "mongodb://localhost:27017",
                        DatabaseName = "test"
                    })
                .AddSingleton<MongoApplicationDbContext>()
                .AddSingleton<ISentUpdatesRepository, MongoSentUpdatesRepository>()
                .BuildServiceProvider();

            _repository = services.GetService<ISentUpdatesRepository>();
        }

        [TestMethod]
        public async Task TestGetSet()
        {
            const string url = "https://test.com";

            for (int i = 0; i < 2; i++)
            {
                if (await _repository.ExistsAsync(url))
                {
                    await _repository.RemoveAsync(url);
                }
                await _repository.AddAsync(url);
            
                Assert.IsTrue(await _repository.ExistsAsync(url));                
            }
        }
    }
}