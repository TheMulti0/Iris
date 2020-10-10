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
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DashboardBackend.Controllers 
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        
        [HttpGet("login")]
        public IActionResult SignInWithExternalProvider(string provider, string returnUrl)
        {
            string callback = Url.Action(
                nameof(HandleExternalLogin),
                new { returnUrl });
            
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties(
                provider,
                callback);
            
            return Challenge(authenticationProperties, provider);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> HandleExternalLogin(string returnUrl)
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            
            // Check if the user has previously logged in using the external login provider.
            // If that’s the case this method will effectively sign the user in.
            SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false); 
                                    
            if (!result.Succeeded) // User does not exist yet
            {
                IdentityUser newUser = CreateUser(info);

                IdentityResult createResult = await _userManager.CreateAsync(newUser);
                if (!createResult.Succeeded)
                {
                    throw new Exception(createResult.Errors.Select(e => e.Description).Aggregate((errors, error) => $"{errors}, {error}"));
                }

                await _userManager.AddLoginAsync(newUser, info);
                
                IEnumerable<Claim> newUserClaims = info.Principal.Claims.Append(new Claim("userId", newUser.Id)); // Generated UUID
                
                await _userManager.AddClaimsAsync(newUser, newUserClaims);
                
                // Sign in with ApplicationScheme / cookies (with the new user information).
                // The cookie expires after browser is closed (not persistent).
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                // Sign out of the ExternalScheme (which contains the information received from the external login provider about the user).
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            return Redirect(returnUrl);                        
        }

        private static IdentityUser CreateUser(ExternalLoginInfo info)
        {
            string name = info.Principal.FindFirstValue(ClaimTypes.Name);
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);
            
            return new IdentityUser
            {
                UserName = name,
                Email = email,
                EmailConfirmed = true
            };
        }
        
        [HttpGet("isAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return new ObjectResult(User.Identity.IsAuthenticated);
        }

        [HttpGet("name")]
        [Authorize]
        public JsonResult Name()
        {
            string givenName = User.FindFirst(ClaimTypes.Name).Value;
            return Json(givenName);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string returnUrl) 
        {
            // Sign out of ApplicationScheme (cookies).
            await _signInManager.SignOutAsync();
            
            return Redirect(returnUrl);
        }
    }
}