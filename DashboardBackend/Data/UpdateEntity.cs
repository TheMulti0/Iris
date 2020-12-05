using System;
using Common;
using MongoDB.Bson.Serialization.Attributes;

namespace DashboardBackend.Data
{
    public class UpdateEntity : Update
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}