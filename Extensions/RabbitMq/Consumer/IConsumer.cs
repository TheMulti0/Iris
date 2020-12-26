using System.Threading;
using System.Threading.Tasks;

namespace Extensions
{
    public interface IConsumer<in T>
    {
        Task ConsumeAsync(T item, CancellationToken token);
    }
}