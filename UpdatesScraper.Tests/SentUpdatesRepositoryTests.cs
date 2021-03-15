using System;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDbGenericRepository;

namespace UpdatesScraper.Tests
{
    [TestClass]
    public class SentUpdatesRepositoryTests
    {
        private static MongoDbConfig _mongoDbConfig;
        private static IMongoDbContext _dbContext;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _mongoDbConfig = new MongoDbConfig
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "test"
            };

            _dbContext = new MongoDbContext(_mongoDbConfig.ConnectionString, _mongoDbConfig.DatabaseName);
        }

        [TestMethod]
        public async Task TestGetSetNoIndexCreation()
        {
            const string url = "https://test.com";

            var repository = new MongoSentUpdatesRepository(_dbContext, _mongoDbConfig);

            await _dbContext.GetCollection<SentUpdate>().Indexes.DropAllAsync();

            repository.CreateExpirationIndex(_mongoDbConfig);
            
            await TestGetSet(url, repository);
        }

        [TestMethod]
        public async Task TestGetSetWithIndexCreation()
        {
            const string url = "https://test.com";

            await _dbContext.GetCollection<SentUpdate>().Indexes.DropAllAsync();
            
            await TestGetSet(url, new MongoSentUpdatesRepository(
                                      _dbContext,
                                      _mongoDbConfig));
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