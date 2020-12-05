using System;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdatesProducer
{
    public class SentUpdate
    {
        public DateTime CreatedAt { get; set; }
        
        [BsonId]
        public string Url { get; set; }
    }
}