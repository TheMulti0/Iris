using System;
using System.Threading.Tasks;

namespace UpdatesScraper
{
    public interface IUserLatestUpdateTimesRepository
    {
        Task<UserLatestUpdateTime> GetAsync(string userId);
        
        Task AddOrUpdateAsync(string userId, DateTime latestUpdateTime);    
    }
}