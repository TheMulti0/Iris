using System;
using Newtonsoft.Json;

namespace FacebookScraper
{
    public record Comment
    {
        [JsonProperty("comment_id")]
        public string CommentId { get; init; }

        [JsonProperty("comment_image")]
        public string CommentImageUrl { get; init; }

        [JsonProperty("comment_text")]
        public string CommentText { get; init; }
        
        [JsonProperty("comment_time")]
        public TimeSpan CommentTime { get; init; }

        [JsonProperty("comment_url")]
        public string CommentUrl { get; init; }

        [JsonProperty("commenter_id")]
        public string CommenterId { get; init; }

        [JsonProperty("commenter_meta")]
        public object CommenterMeta { get; init; }

        [JsonProperty("commenter_name")]
        public string CommenterName { get; init; }

        [JsonProperty("commenter_url")]
        public string CommenterUrl { get; init; }
    }
}