using System.Collections.Generic;
using System.Threading.Tasks;
using DashboardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DashboardBackend.Controllers
{
    [Route("[controller]")]
    public class MeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public MeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet]
        [Authorize]
        public Task<ApplicationUser> Me()
        {
            return _userManager.GetUserAsync(User);
        }
        
        [HttpGet("roles")]
        [Authorize]
        public async Task<IList<string>> Roles()
        {
            ApplicationUser applicationUser = await Me();
            
            return await _userManager.GetRolesAsync(applicationUser);
        }
    }
}