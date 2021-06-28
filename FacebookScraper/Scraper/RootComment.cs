using Newtonsoft.Json;

namespace FacebookScraper
{
    public record RootComment : Comment
    {
        [JsonProperty("replies")]
        public Comment[] Replies { get; init; }
    }
}