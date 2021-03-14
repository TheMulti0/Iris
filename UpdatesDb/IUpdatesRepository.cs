using System.Threading.Tasks;
using Common;
using MongoDB.Bson;

namespace UpdatesDb
{
    public interface IUpdatesRepository
    {
        Slice<UpdateEntity> Get(int startIndex, int limit);
        
        Task<UpdateEntity> GetAsync(ObjectId id);
        
        Task AddOrUpdateAsync(UpdateEntity entity);

    }
}