using System;
using TdLib;

namespace TelegramClient
{
    public class InputMessageContentFileStream : TdApi.InputMessageContent
    {
        public TdApi.InputMessageContent InputMessageContent { get; init; }
        
        public IAsyncDisposable InputFileStream { get; init; }
    }
}