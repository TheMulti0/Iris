using System.Threading.Tasks;
using Common;
using MongoDB.Bson;

namespace UpdatesDb
{
    public interface IUpdatesRepository
    {
        Paged<UpdateEntity> Get(int pageIndex, int pageSize);
        
        Task<UpdateEntity> GetAsync(ObjectId id);
        
        Task AddOrUpdateAsync(UpdateEntity entity);

    }
}