using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramConsumer
{
    public interface ITelegramBotClientProvider
    {
        Task<ITelegramBotClient> CreateAsync(TelegramConfig config);
    }
}