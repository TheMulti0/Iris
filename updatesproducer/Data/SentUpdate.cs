using System;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdatesProducer
{
    public class SentUpdate
    {
        [BsonId]
        public string Url { get; set; }

        public DateTime SentAt { get; set; }
    }
}