using System;
using System.Text.Json.Serialization;

namespace Iris.Twitter
{
    internal class Tweet
    {
        [JsonPropertyName("entries")]
        public Media Media { get; set; }
        
        [JsonPropertyName("isRetweet")]
        public bool IsRetweet { get; set; }

        [JsonPropertyName("likes")]
        public int Likes { get; set; }

        [JsonPropertyName("replies")]
        public int Replies { get; set; }

        [JsonPropertyName("retweets")]
        public int Retweets { get; set; }
        
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("time")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("tweetId")]
        [JsonConverter(typeof(StringToLongConverter))]
        public long Id { get; set; }
    }
}