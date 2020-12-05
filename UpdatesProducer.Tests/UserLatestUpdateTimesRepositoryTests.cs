using System;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdatesProducer.Tests
{
    [TestClass]
    public class UserLatestUpdateTimesRepositoryTests
    {
        private static IUserLatestUpdateTimesRepository _repository;

        public UserLatestUpdateTimesRepositoryTests()
        {
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddTestsLogging(context))
                .AddMongoDb(
                    new MongoDbSettings
                    {
                        ConnectionString = "mongodb://localhost:27017",
                        DatabaseName = "test"
                    })
                .AddSingleton<ApplicationDbContext>()
                .AddSingleton<IUserLatestUpdateTimesRepository, UserLatestUpdateTimesRepository>()
                .BuildServiceProvider();

            _repository = services.GetService<IUserLatestUpdateTimesRepository>();
        }

        [TestMethod]
        public async Task TestGetSet()
        {
            const string userId = "test";
            DateTime latestUpdateTime = DateTime.Parse(DateTime.Now.ToString());
            
            await _repository.SetAsync(userId, latestUpdateTime);
            
            UserLatestUpdateTime userLatestUpdateTime = await _repository.GetAsync(userId);
            
            Assert.AreEqual(userId, userLatestUpdateTime.UserId);
            Assert.AreEqual(latestUpdateTime, userLatestUpdateTime.LatestUpdateTime.ToLocalTime());
        }
    }
}