using System;
using Common;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdatesScraper
{
    public class UserLatestUpdateTime
    {
        public User User { get; set; }

        public int Version { get; set; }
        
        public DateTime LatestUpdateTime { get; set; }
    }
}