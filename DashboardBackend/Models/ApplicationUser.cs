using AspNetCore.Identity.MongoDbCore.Models;

namespace DashboardBackend.Models
{
    public class ApplicationUser : MongoIdentityUser
    {
        public string ProfilePicture { get; set; }   
    }
}