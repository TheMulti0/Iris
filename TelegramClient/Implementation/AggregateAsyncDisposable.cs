using System;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramClient
{
    public class AggregateAsyncDisposable : IAsyncDisposable
    {
        private readonly object[] _disposables;

        public AggregateAsyncDisposable(
            params object[] disposables)
        {
            _disposables = disposables;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (IAsyncDisposable asyncDisposable in _disposables.OfType<IAsyncDisposable>())
            {
                await asyncDisposable.DisposeAsync();
            }
            foreach (IDisposable disposable in _disposables.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }
    }
}