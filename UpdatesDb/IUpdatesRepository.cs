using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;

namespace UpdatesDb
{
    public interface IUpdatesRepository
    {
        Task<bool> ExistsAsync(User user);
        
        IQueryable<UpdateEntity> Get();
        
        Task<UpdateEntity> GetAsync(ObjectId id);
        
        Task AddOrUpdateAsync(UpdateEntity entity);

        Task RemoveAsync(User user, string chatId);
    }
}