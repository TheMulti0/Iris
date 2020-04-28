using System;
using Newtonsoft.Json;

namespace Iris.Facebook
{
    public class Post
    {
        [JsonProperty("comments")]
        public int Comments { get; set; }

        [JsonProperty("image")]
        public string ImageUrl { get; set; }

        [JsonProperty("likes")]
        public int Likes { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("post_id")]
        public long Id { get; set; }

        [JsonProperty("post_text")]
        public string PostText { get; set; }

        [JsonProperty("post_url")]
        public string PostUrl { get; set; }

        [JsonProperty("shared_text")]
        public string SharedText { get; set; }

        [JsonProperty("shares")]
        public int Shares { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("time")]
        public DateTime Date { get; set; }
    }
}