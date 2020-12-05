using System.Threading.Tasks;

namespace UpdatesProducer
{
    public interface ISentUpdatesRepository
    {
        Task<SentUpdate> GetAsync(string url);
        
        Task SetAsync(string url);
    }
}