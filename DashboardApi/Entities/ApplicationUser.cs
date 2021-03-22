using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;

namespace DashboardApi
{
    public class ApplicationUser : MongoIdentityUser<ObjectId>
    {
        public string ProfilePicture { get; set; }   
    }
}