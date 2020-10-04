using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdatesConsumer
{
    public class Update
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("author_id")]
        public string AuthorId { get; set; }

        [JsonPropertyName("creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("media")] 
        public List<IMedia> Media { get; set; }

        [JsonPropertyName("repost")]
        public bool Repost { get; set; }

        public override string ToString()
        {
            return $"Url: {Url} - Creation date: {CreationDate} - Media length: {Media.Count} - Repost: {Repost}";
        }
    }
}