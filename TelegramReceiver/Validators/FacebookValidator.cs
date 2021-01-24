using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using FacebookScraper;

namespace TelegramReceiver
{
    internal class FacebookValidator : IPlatformValidator
    {
        private const string FacebookUserNamePattern = @"(https?:\/\/(www\.)?(m.)?facebook.com\/)?(?<userName>[\w\d.]+)";
        private static readonly Regex FacebookUserNameRegex = new(FacebookUserNamePattern);
        
        private readonly FacebookUpdatesProvider _facebook;

        public FacebookValidator(FacebookUpdatesProvider facebook)
        {
            _facebook = facebook;
        }

        public async Task<User> ValidateAsync(User request)
        {
            Group group = FacebookUserNameRegex.Match(request.UserId)?.Groups["userName"];

            if (!group.Success)
            {
                return null;
            }

            User newUser = request with { UserId = group.Value };
            
            IEnumerable<Update> updates = await _facebook.GetUpdatesAsync(newUser);

            if (updates == null || !updates.Any())
            {
                return null;
            }

            return newUser;
        }
    }
}