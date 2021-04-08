using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TelegramClient
{
    internal class RemoteFileStream
    {
        private static readonly HttpClient HttpClient = new();

        private readonly string _remoteUrl;

        public RemoteFileStream(string remoteUrl)
        {
            _remoteUrl = remoteUrl;
        }
        
        public async Task<Stream> GetStreamAsync() => await HttpClient.GetStreamAsync(_remoteUrl);
    }
}