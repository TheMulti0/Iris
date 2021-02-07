using System.Threading.Tasks;
using User = Telegram.Bot.Types.User;

namespace TelegramReceiver
{
    public interface IConnectionsRepository
    {
        Task<Connection> GetAsync(User user);
        
        Task AddOrUpdateAsync(User user, IConnectionProperties properties);
    }
}