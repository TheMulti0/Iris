using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramBot
{
    public interface ITelegramBotClientProvider
    {
        Task<ITelegramBotClient> CreateAsync(TelegramConfig config);
    }
}