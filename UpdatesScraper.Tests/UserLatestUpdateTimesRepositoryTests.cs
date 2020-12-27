using System;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdatesScraper.Tests
{
    [TestClass]
    public class UserLatestUpdateTimesRepositoryTests
    {
        private static IUserLatestUpdateTimesRepository _repository;

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
                .AddSingleton<IUserLatestUpdateTimesRepository, MongoUserLatestUpdateTimesRepository>()
                .BuildServiceProvider();

            _repository = services.GetService<IUserLatestUpdateTimesRepository>();
        }

        [TestMethod]
        public async Task TestGetSet()
        {
            var userId = new User("test", null, "test");
            DateTime latestUpdateTime = DateTime.Parse(DateTime.Now.ToString()); // To ignore millisecond precision
            
            await _repository.AddOrUpdateAsync(userId, latestUpdateTime);
            
            UserLatestUpdateTime userLatestUpdateTime = await _repository.GetAsync(userId);
            
            Assert.AreEqual(userId, userLatestUpdateTime.User);
            Assert.AreEqual(latestUpdateTime, userLatestUpdateTime.LatestUpdateTime.ToLocalTime());
        }
    }
}