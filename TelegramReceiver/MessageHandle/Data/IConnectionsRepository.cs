using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using User = Telegram.Bot.Types.User;

namespace TelegramReceiver.Data
{
    public interface IConnectionsRepository
    {
        Task<Connection> GetAsync(User user);
        
        Task AddOrUpdateAsync(User user, ChatId chatId, Language language);
    }
}