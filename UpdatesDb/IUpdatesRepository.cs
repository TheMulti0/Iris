using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace UpdatesDb
{
    public interface IUpdatesRepository
    {
        IQueryable<UpdateEntity> Get();
        
        Task<UpdateEntity> GetAsync(ObjectId id);
        
        Task AddOrUpdateAsync(UpdateEntity entity);

    }
}