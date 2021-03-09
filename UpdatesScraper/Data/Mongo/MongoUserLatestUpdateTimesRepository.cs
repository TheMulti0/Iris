using System;
using System.Threading.Tasks;
using Common;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbGenericRepository;

namespace UpdatesScraper
{
    public class MongoUserLatestUpdateTimesRepository : IUserLatestUpdateTimesRepository
    {
        private readonly IMongoCollection<UserLatestUpdateTime> _collection;

        public MongoUserLatestUpdateTimesRepository(IMongoDbContext context)
        {
            _collection = context.GetCollection<UserLatestUpdateTime>();
        }
        
        public Task<UserLatestUpdateTime> GetAsync(User user)
        {
            (string userId, Platform platform) = user;
            
            return _collection
                .AsQueryable()
                .FirstOrDefaultAsync(u => u.User.UserId == userId &&
                                          u.User.Platform == platform);
        }

        public async Task AddOrUpdateAsync(User user, DateTime latestUpdateTime)
        {
            var updateTime = new UserLatestUpdateTime
            {
                User = user,
                LatestUpdateTime = latestUpdateTime
            };

            var existing = await GetAsync(user);
            
            if (existing == null)
            {
                await _collection.InsertOneAsync(updateTime);
                return;
            }

            bool updateSuccess;
            do
            {
                existing = await GetAsync(user);

                UpdateDefinition<UserLatestUpdateTime> update = Builders<UserLatestUpdateTime>.Update
                    .Set(u => u.Version, existing.Version + 1)
                    .Set(u => u.LatestUpdateTime, latestUpdateTime);
                
                var result = await _collection
                    .UpdateOneAsync(
                        userLatestUpdateTime => userLatestUpdateTime.Version == existing.Version &&
                                                userLatestUpdateTime.User == user,
                        update);

                updateSuccess = result.IsAcknowledged && result.ModifiedCount > 0;
            }
            while (!updateSuccess);
        }
    }
}