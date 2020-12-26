using System;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdatesScraper
{
    public class SentUpdate
    {
        [BsonId]
        public string Url { get; set; }

        public int Version { get; set; }

        public DateTime SentAt { get; set; }
    }
}