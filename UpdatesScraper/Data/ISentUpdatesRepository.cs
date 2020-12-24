using System.Threading.Tasks;

namespace UpdatesScraper
{
    public interface ISentUpdatesRepository
    {
        Task<bool> ExistsAsync(string url);

        Task AddAsync(string url);
    }
}