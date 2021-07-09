using System;
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

        [JsonIgnore]
        public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);

        [JsonProperty("timeout")]
        public int TimeoutSeconds => (int) Timeout.TotalSeconds;
        
        [JsonProperty("cookies_filename")]
        public string CookiesFileName { get; init; }
    }
}