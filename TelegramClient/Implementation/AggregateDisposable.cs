using System;

namespace TelegramClient
{
    public class AggregateDisposable : IDisposable
    {
        private readonly IDisposable[] _disposeAsyncMethods;

        public AggregateDisposable(
            params IDisposable[] disposeAsyncMethods)
        {
            _disposeAsyncMethods = disposeAsyncMethods;
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in _disposeAsyncMethods)
            {
                disposable.Dispose();
            }
        }
    }
}