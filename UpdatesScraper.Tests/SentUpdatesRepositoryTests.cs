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
        private static MongoApplicationDbContext _dbContext;
        private static MongoDbConfig _config;

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
                .BuildServiceProvider();

            _dbContext = services.GetService<MongoApplicationDbContext>();
            _config = services.GetService<MongoDbConfig>();
        }

        [TestMethod]
        public async Task TestGetSetNoIndexCreation()
        {
            const string url = "https://test.com";

            var repository = new MongoSentUpdatesRepository(
                _dbContext,
                _config);

            await _dbContext.SentUpdates.Indexes.DropAllAsync();

            repository.CreateExpirationIndex(_config);
            
            await TestGetSet(url, repository);
        }

        [TestMethod]
        public async Task TestGetSetWithIndexCreation()
        {
            const string url = "https://test.com";

            await _dbContext.SentUpdates.Indexes.DropAllAsync();
            
            await TestGetSet(url, new MongoSentUpdatesRepository(
                                      _dbContext,
                                      _config));
        }

        private static async Task TestGetSet(string url, ISentUpdatesRepository repository)
        {
            for (int i = 0; i < 2; i++)
            {
                if (await repository.ExistsAsync(url))
                {
                    await repository.RemoveAsync(url);
                }
                await repository.AddAsync(url);

                Assert.IsTrue(await repository.ExistsAsync(url));
            }
        }
    }
}