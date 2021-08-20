using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Scraper.Net;

namespace TelegramReceiver
{
    public class TwitterValidator : IPlatformValidator
    {
        private const string TwitterUserNamePattern = @"(https?:\/\/(www\.)?(m.)?twitter.com\/)?@?(?<userName>[\w\d-_]+)";
        private const string PlatformName = "twitter";
        private static readonly Regex TwitterUserNameRegex = new(TwitterUserNamePattern);
        
        private readonly IScraperService _service;

        public TwitterValidator(IScraperService service)
        {
            _service = service;
        }

        public async Task<User> ValidateAsync(string userId)
        {
            Group group = TwitterUserNameRegex.Match(userId)?.Groups["userName"];

            if (!group.Success)
            {
                return null;
            }

            User newUser = new User(group.Value.ToLower(), Platform.Twitter);
            
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