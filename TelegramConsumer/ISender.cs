using System.Threading.Tasks;

namespace TelegramConsumer
{
    public interface ISender
    {
        Task SendAsync(Update update);
    }
}