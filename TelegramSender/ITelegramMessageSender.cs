using System.Threading;
using System.Threading.Tasks;
using Common;

namespace TelegramSender
{
    public interface ITelegramMessageSender
    {
        Task ConsumeAsync(SendMessage message, CancellationToken ct);

        Task FlushAsync();
    }
}