using Newtonsoft.Json;

namespace FacebookScraper
{
    internal record GetPostsResponse
    {
        [JsonProperty("posts")]
        public PostRaw[] Posts { get; init; }

        [JsonProperty("error")]
        public string Error { get; init; }

        [JsonIgnore]
        internal GetPostsRequest OriginalRequest { get; init; }
    }
}