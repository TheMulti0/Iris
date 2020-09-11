using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramConsumer
{
    public static class HttpClientExtensions
    {
        public static async Task<Stream> GetStreamAsync(
            this HttpClient client,
            string url,
            CancellationToken cancellationToken)
        {
            var response = await client.GetAsync(url, cancellationToken);
            return await response.Content.ReadAsStreamAsync();
        }
    }
}