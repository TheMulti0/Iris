using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UpdatesScraper.Mock
{
    public class MockUserLatestUpdateTimesRepository : IUserLatestUpdateTimesRepository
    {
        private readonly ILogger<MockUserLatestUpdateTimesRepository> _logger;

        public MockUserLatestUpdateTimesRepository(
            ILogger<MockUserLatestUpdateTimesRepository> logger)
        {
            _logger = logger;
        }

        public Task<UserLatestUpdateTime> GetAsync(string userId)
        {
            return Task.FromResult(
                new UserLatestUpdateTime
                {
                    UserId = userId,
                    LatestUpdateTime = DateTime.MinValue
                });
        }

        public Task AddOrUpdateAsync(string userId, DateTime latestUpdateTime)
        {
            return Task.CompletedTask;
        }
    }
}