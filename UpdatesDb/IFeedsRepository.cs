using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;

namespace UpdatesDb
{
    public interface IFeedsRepository
    {
        IQueryable<FeedEntity> Get();
        
        Task<FeedEntity> GetAsync(ObjectId id);
        
        Task AddOrUpdateAsync(FeedEntity entity);
    }
}