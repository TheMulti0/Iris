using System;
using System.Threading.Tasks;
using Common;

namespace UpdatesScraper
{
    public interface IUserLatestUpdateTimesRepository
    {
        Task<UserLatestUpdateTime> GetAsync(User user);
        
        Task AddOrUpdateAsync(User user, DateTime latestUpdateTime);    
    }
}