using System;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdatesProducer
{
    public class UserLatestUpdateTime
    {
        [BsonId] 
        public string UserId { get; set; }
        
        public DateTime LatestUpdateTime { get; set; }
    }
}