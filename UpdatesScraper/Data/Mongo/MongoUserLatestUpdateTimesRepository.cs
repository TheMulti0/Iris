using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace UpdatesScraper
{
    public class MongoUserLatestUpdateTimesRepository : IUserLatestUpdateTimesRepository
    {
        private readonly IMongoCollection<UserLatestUpdateTime> _collection;

        public MongoUserLatestUpdateTimesRepository(MongoApplicationDbContext context)
        {
            _collection = context.UserLatestUpdateTimes;
        }
        
        public Task<UserLatestUpdateTime> GetAsync(string userId)
        {
            return _collection
                .AsQueryable()
                .FirstOrDefaultAsync(userLatestUpdateTime => userLatestUpdateTime.UserId == userId);
        }

        public async Task AddOrUpdateAsync(string userId, DateTime latestUpdateTime)
        {
            var updateTime = new UserLatestUpdateTime
            {
                UserId = userId,
                LatestUpdateTime = latestUpdateTime
            };

            var existing = await GetAsync(userId);
            
            if (existing == null)
            {
                await _collection.InsertOneAsync(updateTime);
                return;
            }

            bool updateSuccess;
            do
            {
                existing = await GetAsync(userId);

                UpdateDefinition<UserLatestUpdateTime> update = Builders<UserLatestUpdateTime>.Update
                    .Set(u => u.Version, existing.Version + 1)
                    .Set(u => u.LatestUpdateTime, latestUpdateTime);
                
                var result = await _collection
                    .UpdateOneAsync(
                        userLatestUpdateTime => userLatestUpdateTime.Version == existing.Version &&
                                                userLatestUpdateTime.UserId == userId,
                        update);

                updateSuccess = result.IsAcknowledged && result.ModifiedCount > 0;
            }
            while (!updateSuccess);
        }
    }
}