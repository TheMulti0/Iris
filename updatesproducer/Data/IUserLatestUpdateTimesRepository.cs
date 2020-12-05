using System;
using System.Threading.Tasks;

namespace UpdatesProducer
{
    public interface IUserLatestUpdateTimesRepository
    {
        Task<UserLatestUpdateTime> GetAsync(string userId);
        
        Task SetAsync(string userId, DateTime latestUpdateTime);    
    }
}