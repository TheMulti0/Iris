using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common
{
    public record Update
    {
        [JsonPropertyName("content")]
        public string Content { get; init; }

        [JsonPropertyName("author_id")]
        public string AuthorId { get; init; }

        [JsonPropertyName("creation_date")]
        public DateTime? CreationDate { get; init; }

        [JsonPropertyName("url")]
        public string Url { get; init; }

        [JsonPropertyName("media")] 
        public List<IMedia> Media { get; init; }

        [JsonPropertyName("repost")]
        public bool Repost { get; init; }

        public override string ToString()
        {
            return $"Url: {Url} - Creation date: {CreationDate} - Media length: {Media.Count} - Repost: {Repost}";
        }
    }
}