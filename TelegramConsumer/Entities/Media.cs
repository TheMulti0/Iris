using System.Text.Json.Serialization;

namespace TelegramConsumer
{
    public class Media
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("type")]
        public MediaType Type { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        
        [JsonPropertyName("duration_seconds")]
        public int DurationSeconds { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }
}