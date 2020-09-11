using System.Text.Json.Serialization;

namespace TelegramConsumer
{
    public class Photo : IMedia
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }
}