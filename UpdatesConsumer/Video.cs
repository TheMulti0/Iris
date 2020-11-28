using System.Text.Json.Serialization;

namespace UpdatesConsumer
{
    public class Video : IMedia
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = nameof(Video);

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        
        [JsonPropertyName("duration_seconds")]
        public int? DurationSeconds { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }
}