using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TwitterScraper
{
    public class UrlExpander
    {
        private readonly HttpClient _httpClient;

        public UrlExpander()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> ExpandAsync(string url)
        {
            string result = await _httpClient.GetStringAsync($"{TwitterConstants.LinkunshortenBaseUrl}/link?url={url}");

            var response = JsonSerializer.Deserialize<LinkUnshortenResponse>(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (response == null ||
                response.RedirectUrl == TwitterConstants.FacebookIncorrectRedirectUrl)
            {
                return url;
            }

            string[] possibleUrls = 
            {
                response.RedirectUrl,
                response.Title
            };
            
            return possibleUrls.FirstOrDefault(u => u != null && u != TwitterConstants.TwitterBaseDomain);
        }
    }
}