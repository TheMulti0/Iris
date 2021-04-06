using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public sealed class InputFileStream : IAsyncDisposable
    {
        private readonly Func<Task<Stream>> _getStreamAsync;
        private static readonly HttpClient HttpClient = new();
        private static readonly Random Random = new();

        private readonly string _filePath;
        private readonly FileStream _fileStream;

        public InputFileStream(
            Func<Task<Stream>> getStreamAsync)
        {
            _getStreamAsync = getStreamAsync;
            
            _filePath = GetFilePath();
            _fileStream = new FileStream(_filePath, FileMode.Create);
        }

        private static string GetFilePath()
        {
            long currentTime = new DateTimeOffset().ToUnixTimeSeconds();
            int random = Random.Next();
            
            return $"{currentTime}-{random}";
        }

        public async Task<TdApi.InputFile> GetFileAsync()
        {
            await using Stream remoteStream = await _getStreamAsync();
            
            await remoteStream.CopyToAsync(_fileStream);

            return new TdApi.InputFile.InputFileLocal
            {
                Path = _fileStream.Name
            };
        }

        public async ValueTask DisposeAsync()
        {
            await _fileStream.DisposeAsync();

            File.Delete(_filePath);
        }
    }
}