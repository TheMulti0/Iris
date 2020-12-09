using System;
using Newtonsoft.Json;

namespace FacebookProducer
{
    public record Post(
        [JsonProperty("post_id")]
            string PostId,
        [JsonProperty("text")]
            string Text,
        [JsonProperty("post_text")]
            string PostText,
        [JsonProperty("shared_text")]
            string SharedText,
        [JsonProperty("time")]
            DateTime CreationDate,
        [JsonProperty("image")]
            string ImageUrl,
        [JsonProperty("video")]
            string VideoUrl,
        [JsonProperty("video_thumbnail")]
            string VideoThumbnailUrl,
        [JsonProperty("video_id")]
            string VideoId,
        [JsonProperty("comments")]
            int Comments,
        [JsonProperty("shares")]
            int Shares,
        [JsonProperty("post_url")]
            string PostUrl,
        [JsonProperty("link")]
            string Link,
        [JsonProperty("user_id")]
            string UserId,
        [JsonProperty("author_id")]
            string AuthorId,
        [JsonProperty("images")]
            string[] Images);
}