using System.Collections.Generic;
using System.Threading.Tasks;
using UpdatesConsumer;

namespace DataLayer
{
    public interface IUpdatesRepository
    {
        Task<IEnumerable<Update>> GetAsync();
        
        Task AddAsync(Update update);
        
        Task RemoveAsync(Update update);
    }
}