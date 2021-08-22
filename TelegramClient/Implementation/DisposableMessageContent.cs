using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public class DisposableMessageContent : IAsyncDisposable
    {
        public TdApi.InputMessageContent Content { get; }

        private readonly IDisposable _disposable;
        private readonly IAsyncDisposable _asyncDisposable;

        public DisposableMessageContent(TdApi.InputMessageContent content)
        {
            Content = content;
        }
        
        public DisposableMessageContent(TdApi.InputMessageContent content, IDisposable disposable)
        {
            Content = content;
            _disposable = disposable;
        }
        
        public DisposableMessageContent(TdApi.InputMessageContent content, IAsyncDisposable asyncDisposable)
        {
            Content = content;
            _asyncDisposable = asyncDisposable;
        }

        public async ValueTask DisposeAsync()
        {
            _disposable?.Dispose();
            
            if (_asyncDisposable == null)
            {
                return;
            }

            await _asyncDisposable.DisposeAsync();
        }
    }
}