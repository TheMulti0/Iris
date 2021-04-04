using System;
using Newtonsoft.Json;

namespace FacebookScraper
{
    public record Post 
    {
        [JsonProperty("post_id")]
        public string PostId { get; init; }
        
        [JsonProperty("text")]
        public string Text { get; init; }
        
        [JsonProperty("post_text")]
        public string PostText { get; init; }
        
        [JsonProperty("shared_text")]
        public string SharedText { get; init; }
        
        [JsonProperty("time")]
        public DateTime? CreationDate { get; init; }
        
        [JsonProperty("image")]
        public string ImageUrl { get; init; }
        
        [JsonProperty("video")]
        public string VideoUrl { get; init; }
        
        [JsonProperty("video_thumbnail")]
        public string VideoThumbnailUrl { get; init; }
        
        [JsonProperty("video_id")]
        public string VideoId { get; init; }
        
        [JsonProperty("comments")]
        public int Comments { get; init; }
        
        [JsonProperty("shares")]
        public int Shares { get; init; }
        
        [JsonProperty("post_url")]
        public string PostUrl { get; init; }
        
        [JsonProperty("link")]
        public string Link { get; init; }
        
        [JsonProperty("user_id")]
        public string UserId { get; init; }
        
        [JsonProperty("images")]
        public string[] Images { get; init; }
        
        [JsonProperty("is_live")]
        public bool IsLive { get; init; }

        [JsonProperty("username")]
        public string UserName { get; init; }

        [JsonProperty("factcheck")]
        public object FactCheck { get; init; }

        [JsonProperty("shared_post_id")]
        public string SharedPostId { get; init; }

        [JsonProperty("shared_time")]
        public DateTime? SharedCreationDate { get; init; }

        [JsonProperty("shared_user_id")]
        public string SharedUserId { get; init; }

        [JsonProperty("shared_username")]
        public string SharedUserName { get; init; }

        [JsonProperty("shared_post_url")]
        public string SharedPostUrl { get; init; }

        [JsonProperty("available")]
        public bool Available { get; init; }

        [JsonProperty("comments_full")]
        public object CommentsFull { get; init; }
    }
}