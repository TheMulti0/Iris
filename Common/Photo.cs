using System.Text.Json.Serialization;

namespace Common
{
    public class Photo : IMedia
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = nameof(Photo);
    
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }
}