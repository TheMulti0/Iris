using System;
using System.Threading.Tasks;
using Common;
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

        public Task<UserLatestUpdateTime> GetAsync(User user)
        {
            return Task.FromResult(
                new UserLatestUpdateTime
                {
                    User = user,
                    LatestUpdateTime = DateTime.MinValue
                });
        }

        public Task AddOrUpdateAsync(User user, DateTime latestUpdateTime)
        {
            return Task.CompletedTask;
        }
    }
}