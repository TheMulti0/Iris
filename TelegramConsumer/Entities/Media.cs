using System.Text.Json.Serialization;

namespace TelegramConsumer
{
    public class Media
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("type")]
        public MediaType Type { get; set; }
    }
}