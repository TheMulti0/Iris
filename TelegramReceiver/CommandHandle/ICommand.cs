using System.Threading;
using System.Threading.Tasks;

namespace TelegramReceiver
{
    public interface ICommand
    {
        Task<IRedirectResult> ExecuteAsync(CancellationToken token);
    }
}