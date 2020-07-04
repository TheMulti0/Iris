using System;
using System.Text.Json.Serialization;

namespace TelegramConsumer
{
    public class Update
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}