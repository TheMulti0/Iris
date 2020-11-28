using System.Text.Json.Serialization;

namespace UpdatesConsumer
{
    public interface IMedia
    {
        public string Type { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }
}