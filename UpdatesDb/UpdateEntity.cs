using Common;
using MongoDB.Bson;

namespace UpdatesDb
{
    public record UpdateEntity : Update
    {
        public ObjectId Id { get; set; }
    }
}