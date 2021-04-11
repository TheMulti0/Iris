using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public class DisposableMessageContent : IAsyncDisposable
    {
        public TdApi.InputMessageContent Content { get; }

        private readonly IAsyncDisposable _disposable;

        public DisposableMessageContent(TdApi.InputMessageContent content)
        {
            Content = content;
        }
        
        public DisposableMessageContent(TdApi.InputMessageContent content, IAsyncDisposable disposable)
        {
            Content = content;
            _disposable = disposable;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposable == null)
            {
                return;
            }

            await _disposable.DisposeAsync();
        }
    }
}