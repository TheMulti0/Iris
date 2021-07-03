using Newtonsoft.Json;

namespace FacebookScraper
{
    internal record GetPostsResponse
    {
        [JsonProperty("posts")]
        public PostRaw[] Posts { get; init; }

        [JsonProperty("error")]
        public string Error { get; init; }
        
        [JsonProperty("error_description")]
        public string ErrorDescription { get; init; }

        [JsonIgnore]
        internal GetPostsRequest OriginalRequest { get; init; }
    }
}