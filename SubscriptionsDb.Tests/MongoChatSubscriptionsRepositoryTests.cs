using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDbGenericRepository;

namespace SubscriptionsDb.Tests
{
    [TestClass]
    public class MongoChatSubscriptionsRepositoryTests
    {
        private const long TestChat = 11;
        private readonly IChatSubscriptionsRepository _repository;
        private readonly string TestUser = "Test";
        private readonly string TestPlatform = "facebook";

        public MongoChatSubscriptionsRepositoryTests()
        {
            var config = new MongoDbConfig
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "test"
            };
            
            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>()
                .AddSubscriptionsDb(new MongoDbContext(config.ConnectionString, config.DatabaseName))
                .BuildServiceProvider();

            _repository = services.GetService<IChatSubscriptionsRepository>();
        }
        
        [TestMethod]
        public async Task TestAdd()
        {
            var user = TestUser;
            
            await _repository.AddOrUpdateAsync(TestUser, TestPlatform, new UserChatSubscription
            {
                ChatInfo = new ChatInfo
                {
                    Id = TestChat
                },
            });
            
            Assert.IsTrue(await _repository.ExistsAsync(TestUser, TestPlatform));
        }
        
        [TestMethod]
        public async Task TestUpdate()
        {
            var user = TestUser;

            await TestAdd();

            TimeSpan newInterval = TimeSpan.FromDays(1);
            await _repository.AddOrUpdateAsync(TestUser, TestPlatform, new UserChatSubscription
            {
                ChatInfo = new ChatInfo
                {
                    Id = TestChat
                },
                Interval = newInterval
            });

            var subscriptionEntity = await _repository.GetAsync(TestUser, TestPlatform);
            
            Assert.AreEqual(newInterval, subscriptionEntity.Chats.First(subscription => subscription.ChatInfo.Id == TestChat).Interval);
        }
    }
}