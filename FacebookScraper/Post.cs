using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace FacebookScraper
{
    public record Post 
    {
        [JsonPropertyName("post_id")]
        public string PostId { get; init; }
        
        [JsonPropertyName("text")]
        public string Text { get; init; }
        
        [JsonPropertyName("post_text")]
        public string PostText { get; init; }
        
        [JsonPropertyName("shared_text")]
        public string SharedText { get; init; }
        
        [JsonPropertyName("time")]
        public DateTime CreationDate { get; init; }
        
        [JsonPropertyName("image")]
        public string ImageUrl { get; init; }
        
        [JsonPropertyName("video")]
        public string VideoUrl { get; init; }
        
        [JsonPropertyName("video_thumbnail")]
        public string VideoThumbnailUrl { get; init; }
        
        [JsonPropertyName("video_id")]
        public string VideoId { get; init; }
        
        [JsonPropertyName("comments")]
        public int Comments { get; init; }
        
        [JsonPropertyName("shares")]
        public int Shares { get; init; }
        
        [JsonPropertyName("post_url")]
        public string PostUrl { get; init; }
        
        [JsonPropertyName("link")]
        public string Link { get; init; }
        
        [JsonPropertyName("user_id")]
        public string UserId { get; init; }
        
        [JsonPropertyName("images")]
        public string[] Images { get; init; }
        
        [JsonPropertyName("is_live")]
        public bool IsLive { get; init; }

        [JsonPropertyName("username")]
        public string UserName { get; init; }

        [JsonPropertyName("factcheck")]
        public object FactCheck { get; init; }

        [JsonPropertyName("shared_post_id")]
        public string SharedPostId { get; init; }

        [JsonPropertyName("shared_time")]
        public DateTime SharedCreationDate { get; init; }

        [JsonPropertyName("shared_user_id")]
        public string SharedUserId { get; init; }

        [JsonPropertyName("shared_username")]
        public string SharedUserName { get; init; }

        [JsonPropertyName("shared_post_url")]
        public string SharedPostUrl { get; init; }

        [JsonPropertyName("available")]
        public bool Available { get; init; }

        [JsonPropertyName("comments_full")]
        public object CommentsFull { get; init; }
    }
}