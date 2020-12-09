using System.Threading.Tasks;

namespace UpdatesProducer
{
    public interface ISentUpdatesRepository
    {
        Task<bool> ExistsAsync(string url);

        Task AddAsync(string url);
    }
}