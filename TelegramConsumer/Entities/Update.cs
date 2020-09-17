using System;
using System.Text.Json.Serialization;

namespace TelegramConsumer
{
    public class Update
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("author_id")]
        public string AuthorId { get; set; }

        [JsonPropertyName("creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("media")] 
        public IMedia[] Media { get; set; } = new IMedia[0];

        [JsonPropertyName("repost")]
        public bool Repost { get; set; }

        public override string ToString()
        {
            return $"Url: {Url} - Creation date: {CreationDate} - Media length: {Media.Length} - Repost: {Repost}";
        }
    }
}