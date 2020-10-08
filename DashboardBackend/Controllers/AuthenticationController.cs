using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DashboardBackend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DashboardBackend.Controllers 
{
    [Microsoft.AspNetCore.Components.Route("[controller]")]
    public class AuthController : Controller
    {
        private const string DefaultRedirect = "/";

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(
            SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
        }

        public IActionResult Login(
            string provider,
            string returnUrl = DefaultRedirect)
        {
            string redirectUrl = Url.Action(
                nameof(ExternalLoginCallback),
                new { Provider = provider, ReturnUrl = returnUrl });
            
            AuthenticationProperties properties = _signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(
            string provider,
            string returnUrl)
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(provider, provider);
            (var token, var secret) = info.AuthenticationTokens.ExtractTokens();

            var newUser = new ApplicationUser
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            };

            ApplicationUser existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == newUser.Id);

            if (existingUser == null)
            {
                await RegisterUser(newUser, info, token, secret);

                await SignIn(newUser, returnUrl);
            }
            else
            {
                await SignIn(existingUser, returnUrl);
            }

            return Redirect(returnUrl);
    }

        private async Task RegisterUser(
            ApplicationUser user,
            UserLoginInfo info,
            AuthenticationToken token,
            AuthenticationToken secret)
        {
            await _userManager.RegisterUserAsync(
                user,
                info,
                token,
                secret);
        }

        private async Task SignIn(ApplicationUser user, string returnUrl)
        {
            await _signInManager.SignInAsync(
                user,
                new AuthenticationProperties
                {
                    RedirectUri = returnUrl
                });
        }

        public async Task<IActionResult> Logout(
            string returnUrl)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return Redirect(returnUrl);
        }
    }
}