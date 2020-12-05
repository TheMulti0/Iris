using System;
using System.Threading.Tasks;

namespace UpdatesProducer
{
    public interface IUserLatestUpdateTimesRepository
    {
        Task<UserLatestUpdateTime> GetAsync(string userId);
        
        Task AddOrUpdateAsync(string userId, DateTime latestUpdateTime);    
    }
}