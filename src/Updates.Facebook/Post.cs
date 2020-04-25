using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Updates.Facebook
{
    internal class Post
    {
        [JsonPropertyName("comments")]
        public int Comments { get; set; }

        [JsonPropertyName("image")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("likes")]
        public int Likes { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("post_id")]
        [JsonConverter(typeof(StringToLongConverter))]
        public long Id { get; set; }

        [JsonPropertyName("post_text")]
        public string PostText { get; set; }

        [JsonPropertyName("post_url")]
        public string PostUrl { get; set; }

        [JsonPropertyName("shared_text")]
        public string SharedText { get; set; }

        [JsonPropertyName("shares")]
        public int Shares { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("time")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Date { get; set; }
    }
}