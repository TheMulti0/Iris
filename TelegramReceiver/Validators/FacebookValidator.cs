using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Scraper.Net;
using Post = Scraper.Net.Post;

namespace TelegramReceiver
{
    public class FacebookValidator : IPlatformValidator
    {
        private const string FacebookUserNamePattern = @"(https?:\/\/(www\.)?(m.)?facebook.com\/)?(?<userName>[\w\d-%.]+)";
        private const string PlatformName = "facebook";
        private static readonly Regex FacebookUserNameRegex = new(FacebookUserNamePattern);
        
        private readonly IScraperService _service;

        public FacebookValidator(IScraperService service)
        {
            _service = service;
        }

        public async Task<User> ValidateAsync(string userId)
        {
            Group group = FacebookUserNameRegex.Match(userId)?.Groups["userName"];

            if (!group.Success)
            {
                return null;
            }

            User newUser = new User(group.Value.ToLower(), Platform.Facebook);

            var post = await GetPost(newUser);

            return post == null ? null : newUser;
        }

        private async Task<Post> GetPost(User newUser)
        {
            try
            {
                return await _service.GetPostsAsync(newUser.UserId, PlatformName).FirstOrDefaultAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}