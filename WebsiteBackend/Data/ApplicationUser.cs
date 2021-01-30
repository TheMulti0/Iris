using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;

namespace WebsiteBackend
{
    public class ApplicationUser : MongoIdentityUser<BsonObjectId>
    {
    }
}