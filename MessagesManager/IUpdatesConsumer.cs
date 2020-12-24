using System.Threading;
using System.Threading.Tasks;
using Common;

namespace MessagesManager
{
    public interface IUpdatesConsumer
    {
        Task OnUpdateAsync(Update update, CancellationToken token);
    }
}