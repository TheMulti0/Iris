using System.Collections.Generic;
using Common;
using MongoDB.Bson;

namespace UpdatesDb
{
    public record FeedEntity
    {
        public ObjectId Id { get; set; }

        public ObjectId OwnerId { get; set; }

        public string Name { get; set; }

        public List<User> Users { get; set; }
    }
}