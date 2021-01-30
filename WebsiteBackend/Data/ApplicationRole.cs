using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;

namespace WebsiteBackend
{
    public class ApplicationRole : MongoIdentityRole<BsonObjectId>
    {
        public ApplicationRole()
        {
            
        }
        
        public ApplicationRole(string roleName): base(roleName)
        {
            
        }
    }
}