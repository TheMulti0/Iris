using System.Threading.Tasks;

namespace UpdatesProducer
{
    public interface ISentUpdatesRepository
    {
        Task<SentUpdate> GetAsync(string url);
        
        Task AddAsync(string url);
    }
}