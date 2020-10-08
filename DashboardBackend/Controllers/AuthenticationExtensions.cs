using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace DashboardBackend.Controllers
{
    public static class AuthenticationExtensions
    {
        public static async Task<ExternalLoginInfo> GetExternalLoginInfoAsync<TUser>(
            this SignInManager<TUser> manager, 
            string provider,
            string displayName) where TUser : class
        {
            AuthenticateResult auth = await manager.Context.AuthenticateAsync(provider);
            
            IDictionary<string, string> items = auth.Properties.Items;
            if (!auth.Succeeded)
            {
                return null;
            }

            return new ExternalLoginInfo(auth.Principal, provider, provider, displayName)
            {
                AuthenticationTokens = auth.Properties.GetTokens(),
                AuthenticationProperties = auth.Properties
            };
        }
        
        public static async Task RegisterUserAsync<TUser>(
            this UserManager<TUser> userManager,
            TUser user,
            UserLoginInfo loginInfo,
            AuthenticationToken accessToken,
            AuthenticationToken accessTokenSecret) where TUser : class
        {
            await userManager.CreateAsync(user);
            await userManager.UpdateSecurityStampAsync(user);
            await userManager.AddLoginAsync(user, loginInfo);

            string provider = loginInfo.LoginProvider;
            
            await userManager.SetAuthenticationTokenAsync(
                user,
                provider,
                accessToken.Name,
                accessToken.Value);

            await userManager.SetAuthenticationTokenAsync(
                user,
                provider,
                accessTokenSecret.Name,
                accessTokenSecret.Value);
        }

        public static (AuthenticationToken accessToken, AuthenticationToken accessTokenSecret) ExtractTokens(
            this IEnumerable<AuthenticationToken> tokens)
        {
            IEnumerable<AuthenticationToken> tokensList = tokens.ToList();
            
            AuthenticationToken accessToken = tokensList
                .First(token => token.Name == "access_token");
            
            AuthenticationToken accessTokenSecret = tokensList
                .Last(token => token.Name == "access_token_secret");
            
            return (accessToken, accessTokenSecret);
        }
    }
}