using System.Text.Json;
using System.Text.Json.Serialization;

namespace MessagesManager
{
    internal record ExtractVideoResponse
    {
        [JsonPropertyName("video_info")]
        public JsonElement? VideoInfo { get; init; }

        [JsonPropertyName("error")]
        public string Error { get; init; }
        
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; init; }

        [JsonIgnore]
        internal ExtractVideoRequest OriginalRequest { get; init; }
    }
}