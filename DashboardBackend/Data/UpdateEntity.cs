using System;
using Common;
using MongoDB.Bson.Serialization.Attributes;

namespace DashboardBackend.Data
{
    public record UpdateEntity : Update
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}