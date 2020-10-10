using Microsoft.AspNetCore.Identity;

namespace DashboardBackend.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePicture { get; set; }   
    }
}