using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using FacebookScraper;

namespace TelegramReceiver
{
    public class FacebookValidator : IPlatformValidator
    {
        private const string FacebookUserNamePattern = @"(https?:\/\/(www\.)?(m.)?facebook.com\/)?(?<userName>[\w\d-%.]+)";
        private static readonly Regex FacebookUserNameRegex = new(FacebookUserNamePattern);
        
        private readonly FacebookUpdatesProvider _facebook;

        public FacebookValidator(FacebookUpdatesProvider facebook)
        {
            _facebook = facebook;
        }

        public async Task<User> ValidateAsync(string userId)
        {
            Group group = FacebookUserNameRegex.Match(userId)?.Groups["userName"];

            if (!group.Success)
            {
                return null;
            }

            User newUser = new User(group.Value, Platform.Facebook);
            
            IEnumerable<Update> updates = await _facebook.GetUpdatesAsync(newUser);

            if (updates == null || !updates.Any())
            {
                return null;
            }

            return newUser;
        }
    }
}