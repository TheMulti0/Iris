using Microsoft.AspNetCore.Identity;

namespace DashboardBackend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePicture { get; set; }   
    }
}