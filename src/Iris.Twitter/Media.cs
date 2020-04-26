using System.Text.Json.Serialization;

namespace Iris.Twitter
{
    internal class Media
    {
        [JsonPropertyName("hashtags")]
        public string[] Hashtags { get; set; }

        [JsonPropertyName("photos")]
        public string[] Photos { get; set; }

        [JsonPropertyName("urls")]
        public string[] Urls { get; set; }

        // [JsonPropertyName("videos")]
        // public string[] Videos { get; set; }
    }
}