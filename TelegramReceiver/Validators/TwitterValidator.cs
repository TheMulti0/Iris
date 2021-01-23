using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using FacebookScraper;
using TwitterScraper;

namespace TelegramReceiver
{
    internal class TwitterValidator : IPlatformValidator
    {
        private const string TwitterUserNamePattern = @"(https?:\/\/(www\.)?(m.)?twitter.com\/)?(?<userName>[\w\d-_]+)";
        private static readonly Regex TwitterUserNameRegex = new(TwitterUserNamePattern);
        
        private readonly TwitterUpdatesProvider _twitter;

        public TwitterValidator(TwitterUpdatesProvider twitter)
        {
            _twitter = twitter;
        }

        public async Task<User> ValidateAsync(User request)
        {
            Group group = TwitterUserNameRegex.Match(request.UserId)?.Groups["userName"];

            if (!@group.Success)
            {
                return null;
            }

            User newUser = request with { UserId = @group.Value };
            
            IEnumerable<Update> updates = await _twitter.GetUpdatesAsync(newUser);

            if (updates == null || !updates.Any())
            {
                return null;
            }

            return newUser;
        }
    }
}