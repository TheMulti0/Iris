using System;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDbGenericRepository;

namespace UpdatesScraper.Tests
{
    [TestClass]
    public class UserLatestUpdateTimesRepositoryTests
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
        public async Task TestGetSet()
        {
            var userId = new User("test", Platform.Facebook);
            DateTime latestUpdateTime = DateTime.Parse(DateTime.Now.ToString()); // To ignore millisecond precision

            var repository = new MongoUserLatestUpdateTimesRepository(_dbContext);
            
            await repository.AddOrUpdateAsync(userId, latestUpdateTime.Add(TimeSpan.FromDays(1)));
            await repository.AddOrUpdateAsync(userId, latestUpdateTime);
            
            UserLatestUpdateTime userLatestUpdateTime = await repository.GetAsync(userId);
            
            Assert.AreEqual(userId, userLatestUpdateTime.User);
            Assert.AreEqual(latestUpdateTime, userLatestUpdateTime.LatestUpdateTime.ToLocalTime());
        }
    }
}