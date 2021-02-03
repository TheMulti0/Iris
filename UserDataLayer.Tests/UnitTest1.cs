using System;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbGenericRepository;

namespace UserDataLayer.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mongoApplicationDbContext = new MongoApplicationDbContext(
                new MongoDbContext(new MongoClient(new MongoUrl("mongodb://localhost:27017")).GetDatabase("UsersDb")));

            IMongoQueryable<UserChatSubscription> a = mongoApplicationDbContext.SavedUsers.AsQueryable()
                .SelectMany(user => user.Chats);
            var b = a.ToList();
            Console.ReadLine();
        }
    }
}