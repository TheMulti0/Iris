using System;
using System.Threading.Tasks;

namespace TelegramClient
{
    public class AggregateAsyncDisposable : IAsyncDisposable
    {
        private readonly IAsyncDisposable[] _disposeAsyncMethods;

        public AggregateAsyncDisposable(
            params IAsyncDisposable[] disposeAsyncMethods)
        {
            _disposeAsyncMethods = disposeAsyncMethods;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (IAsyncDisposable disposable in _disposeAsyncMethods)
            {
                await disposable.DisposeAsync();
            }
        }
    }
}