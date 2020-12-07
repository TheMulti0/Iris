using System.Text.Json.Serialization;

namespace Common
{
    public class Video : IMedia
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = nameof(Video);

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        
        //TODO use TimeSpan
        [JsonPropertyName("duration_seconds")]
        public int? DurationSeconds { get; set; }

        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonIgnore]
        public bool IsHighestFormatAvaliable { get; set; }
    }
}