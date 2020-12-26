using System;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdatesScraper
{
    public class UserLatestUpdateTime
    {
        [BsonId] 
        public string UserId { get; set; }

        public int Version { get; set; }
        
        public DateTime LatestUpdateTime { get; set; }
    }
}