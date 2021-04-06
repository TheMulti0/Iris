using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    internal class RemoteFileStream : IAsyncDisposable
    {
        private static readonly HttpClient HttpClient = new();

        private readonly string _remoteUrl;
        private readonly InputFileStream _inputFileStream;

        public RemoteFileStream(string remoteUrl)
        {
            _remoteUrl = remoteUrl;

            _inputFileStream = new InputFileStream(GetStreamAsync);
        }
        
        private async Task<Stream> GetStreamAsync() => await HttpClient.GetStreamAsync(_remoteUrl);

        public Task<TdApi.InputFile> GetFileAsync() => _inputFileStream.GetFileAsync();

        public ValueTask DisposeAsync() => _inputFileStream.DisposeAsync();
    }
}