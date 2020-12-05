using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace UpdatesProducer
{
    public class UserLatestUpdateTimesRepository : IUserLatestUpdateTimesRepository
    {
        private readonly IMongoCollection<UserLatestUpdateTime> _collection;

        public UserLatestUpdateTimesRepository(ApplicationDbContext context)
        {
            _collection = context.UserLatestUpdateTimes;
        }
        
        public Task<UserLatestUpdateTime> GetAsync(string userId)
        {
            return _collection
                .AsQueryable()
                .FirstOrDefaultAsync(userLatestUpdateTime => userLatestUpdateTime.UserId == userId);
        }

        public async Task SetAsync(string userId, DateTime latestUpdateTime)
        {
            var updateTime = new UserLatestUpdateTime
            {
                UserId = userId,
                LatestUpdateTime = latestUpdateTime
            };
            
            UserLatestUpdateTime newEntity = await _collection
                .FindOneAndReplaceAsync(
                    userLatestUpdateTime => userLatestUpdateTime.UserId == userId,
                    updateTime);

            if (newEntity == null)
            {
                await _collection.InsertOneAsync(updateTime);
            }
        }
    }
}