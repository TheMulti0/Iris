using System.Text.Json.Serialization;

namespace MessagesManager
{
    internal record ExtractVideoRequest
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
        
        [JsonPropertyName("format")]
        public string Format { get; init; }
        
        [JsonPropertyName("username")]
        public string UserName { get; init; }
        
        [JsonPropertyName("password")]
        public string Password { get; init; }
    }
}