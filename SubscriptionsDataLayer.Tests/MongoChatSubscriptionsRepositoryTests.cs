using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDbGenericRepository;

namespace SubscriptionsDataLayer.Tests
{
    [TestClass]
    public class MongoChatSubscriptionsRepositoryTests
    {
        private const string TestChat = "testChat";
        private readonly IChatSubscriptionsRepository _repository;
        private readonly User TestUser = new User("Test", Platform.Facebook);

        public MongoChatSubscriptionsRepositoryTests()
        {
            var config = new MongoDbConfig
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "test"
            };
            
            var services = new ServiceCollection()
                .AddSingleton<IMongoDbContext>(new MongoDbContext(config.ConnectionString, config.DatabaseName))
                .AddSingleton(config)
                .AddSingleton<MongoApplicationDbContext>()
                .AddSingleton<IChatSubscriptionsRepository, MongoChatSubscriptionsRepository>()
                .BuildServiceProvider();

            _repository = services.GetService<IChatSubscriptionsRepository>();
        }
        
        [TestMethod]
        public async Task TestAdd()
        {
            var user = TestUser;
            
            await _repository.AddOrUpdateAsync(user, new UserChatSubscription
            {
                ChatId = TestChat
            });
            
            Assert.IsTrue(await _repository.ExistsAsync(user));
        }
        
        [TestMethod]
        public async Task TestUpdate()
        {
            var user = TestUser;

            await TestAdd();

            TimeSpan newInterval = TimeSpan.FromDays(1);
            await _repository.AddOrUpdateAsync(user, new UserChatSubscription
            {
                ChatId = TestChat,
                Interval = newInterval
            });

            var subscriptionEntity = await _repository.GetAsync(user);
            
            Assert.AreEqual(newInterval, subscriptionEntity.Chats.FirstOrDefault(subscription => subscription.ChatId == TestChat).Interval);
        }
    }
}