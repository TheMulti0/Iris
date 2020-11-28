using AspNetCore.Identity.MongoDbCore.Models;

namespace DashboardBackend.Models
{
    public class ApplicationRole : MongoIdentityRole
    {
        public ApplicationRole()
        {
            
        }
        
        public ApplicationRole(string roleName): base(roleName)
        {
            
        }
    }
}