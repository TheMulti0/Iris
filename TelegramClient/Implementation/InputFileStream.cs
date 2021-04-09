using System;
using System.IO;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public sealed class InputFileStream : TdApi.InputFile, IAsyncDisposable
    {
        private static readonly Random Random = new();

        private readonly Func<Task<Stream>> _getStreamAsync;
        private readonly string _filePath;
        private readonly FileStream _fileStream;

        public InputFileStream(
            Func<Task<Stream>> getStreamAsync)
        {
            _getStreamAsync = getStreamAsync;
            
            _filePath = CreateUniqueFilePath();
            _fileStream = new FileStream(_filePath, FileMode.Create);
        }

        private static string CreateUniqueFilePath()
        {
            long currentTime = new DateTimeOffset().ToUnixTimeSeconds();
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
            Console.WriteLine($"Disposing {_filePath}");
            
            await _fileStream.DisposeAsync();

            File.Delete(_filePath);
            
            Console.WriteLine($"Successfully disposed {_filePath}");
        }
    }
}