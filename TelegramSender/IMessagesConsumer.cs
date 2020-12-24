using System.Threading;
using System.Threading.Tasks;
using Common;

namespace TelegramSender
{
    public interface IMessagesConsumer
    {
        Task OnMessageAsync(Message message, CancellationToken token);
    }
}