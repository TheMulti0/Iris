using System;
using System.IO;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public sealed class InputRemoteStream : TdApi.InputFile, IAsyncDisposable
    {
        private static readonly Random Random = new();

        private readonly Func<Task<Stream>> _getStreamAsync;
        private readonly string _filePath;
        private readonly FileStream _fileStream;

        public InputRemoteStream(
            Func<Task<Stream>> getStreamAsync)
        {
            _getStreamAsync = getStreamAsync;
            
            _filePath = CreateUniqueFilePath();
            _fileStream = new FileStream(_filePath, FileMode.Create);
        }

        public static string CreateUniqueFilePath()
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            int random = Random.Next();
            
            return $"{currentTime}-{random}";
        }

        public async Task<TdApi.InputFile> CreateLocalInputFileAsync()
        {
            await using Stream remoteStream = await _getStreamAsync();
        
            await remoteStream.CopyToAsync(_fileStream);                
            
            return new InputFileLocal
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