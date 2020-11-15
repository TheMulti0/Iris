using System.Text.Json.Serialization;

namespace UpdatesConsumer
{
    public class Audio : IMedia
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        
        [JsonPropertyName("duration_seconds")]
        public int? DurationSeconds { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("artist")]
        public string Artist { get; set; }
    }
}