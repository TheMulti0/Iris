using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DashboardBackend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tweetinvi;
using Tweetinvi.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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