using System;
using Newtonsoft.Json;

namespace FacebookScraper
{
    internal record PostRaw 
    {
        [JsonProperty("available")]
        public bool Available { get; init; }
        
        [JsonProperty("post_id")]
        public string PostId { get; init; }

        [JsonProperty("text")]
        public string Text { get; init; }
        
        [JsonProperty("post_text")]
        public string PostText { get; init; }
        
        [JsonProperty("time")]
        public DateTime? Time { get; init; }
        
        [JsonProperty("post_url")]
        public string PostUrl { get; init; }
        
        [JsonProperty("is_live")]
        public bool IsLive { get; init; }
        
        [JsonProperty("link")]
        public string Link { get; init; }

        #region Image

        [JsonProperty("image")]
        public string Image { get; init; }
        
        [JsonProperty("image_id")]
        public string ImageId { get; init; }
        
        [JsonProperty("image_ids")]
        public string[] ImageIds { get; init; }
        
        [JsonProperty("image_lowquality")]
        public string ImageLowQuality { get; init; }
        
        [JsonProperty("images")]
        public string[] Images { get; init; }
        
        [JsonProperty("images_description")]
        public string[] ImagesDescription { get; init; }
        
        [JsonProperty("images_lowquality")]
        public string[] ImagesLowQuality { get; init; }
        
        [JsonProperty("images_lowquality_description")]
        public string[] ImagesLowQualityDescription { get; init; }

        #endregion

        #region Video

        [JsonProperty("video")]
        public string VideoUrl { get; init; }

        [JsonProperty("video_duration_seconds")]
        public int? VideoDurationSeconds { get; init; }
        
        [JsonProperty("video_width")]
        public int? VideoWidth { get; init; }
        
        [JsonProperty("video_height")]
        public int? VideoHeight { get; init; }

        [JsonProperty("video_quality")]
        public string VideoQuality { get; init; }
        
        [JsonProperty("video_thumbnail")]
        public string VideoThumbnail { get; init; }
        
        [JsonProperty("video_id")]
        public string VideoId { get; init; }

        [JsonProperty("video_size_MB")]
        public double? VideoSizeMb { get; init; }
        
        [JsonProperty("video_watches")]
        public int? VideoWatches { get; init; }

        #endregion

        #region Author

        [JsonProperty("user_id")]
        public string UserId { get; init; }

        [JsonProperty("user_url")]
        public string UserUrl { get; init; }
        
        [JsonProperty("username")]
        public string UserName { get; init; }

        #endregion

        #region Stats

        [JsonProperty("comments")]
        public int Comments { get; init; }
        
        [JsonProperty("shares")]
        public int Shares { get; init; }
        
        [JsonProperty("likes")]
        public int Likes { get; init; }

        #endregion

        #region Shared

        [JsonProperty("shared_post_id")]
        public string SharedPostId { get; init; }
        
        [JsonProperty("shared_post_url")]
        public string SharedPostUrl { get; init; }
        
        [JsonProperty("shared_text")]
        public string SharedText { get; init; }
        
        [JsonProperty("shared_time")]
        public DateTime? SharedTime { get; init; }

        [JsonProperty("shared_user_id")]
        public string SharedUserId { get; init; }

        [JsonProperty("shared_username")]
        public string SharedUserName { get; init; }

        #endregion
        
        #region Unknown

        [JsonProperty("factcheck")]
        public object FactCheck { get; set; }

        [JsonProperty("reactions")]
        public object Reactions { get; set; }

        [JsonProperty("reactors")]
        public object Reactors { get; set; }

        #endregion

        [JsonProperty("comments_full")]
        public RootComment[] CommentsFull { get; init; }
    }
}