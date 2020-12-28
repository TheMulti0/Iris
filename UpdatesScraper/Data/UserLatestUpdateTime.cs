using System;
using Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdatesScraper
{
    public class UserLatestUpdateTime
    {
        [BsonId]
        public ObjectId _id { get; set; }
    
        public User User { get; set; }

        public int Version { get; set; }
        
        public DateTime LatestUpdateTime { get; set; }
    }
}