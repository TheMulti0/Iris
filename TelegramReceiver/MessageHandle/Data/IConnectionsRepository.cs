using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramReceiver.Data
{
    public interface IConnectionsRepository
    {
        Task<ChatId> GetAsync(User user);
        
        Task AddOrUpdateAsync(User user, ChatId chatId);
    }
}