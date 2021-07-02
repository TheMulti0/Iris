using Newtonsoft.Json;

namespace FacebookScraper
{
    internal record GetPostsRequest
    {
        [JsonProperty("user_id")]
        public string UserId { get; init; }

        [JsonProperty("pages")]
        public int Pages { get; init; }
        
        [JsonProperty("proxy")]
        public string Proxy { get; init; }
        
        [JsonProperty("cookies_filename")]
        public string CookiesFileName { get; init; }
    }
}