using System.Linq;
using DashboardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DashboardBackend.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet]
        [Authorize(Roles = RoleNames.SuperUser)]
        public IQueryable<ApplicationUser> Users()
        {
            return _userManager.Users;
        }
    }
}