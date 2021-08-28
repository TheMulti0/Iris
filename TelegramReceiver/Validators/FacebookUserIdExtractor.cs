using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Scraper.Net;
using Post = Scraper.Net.Post;

namespace TelegramReceiver
{
    public class FacebookUserIdExtractor : IPlatformUserIdExtractor
    {
        private const string FacebookUserNamePattern = @"(https?:\/\/(www\.)?(m.)?facebook.com\/)?(?<userName>[\w\d-%.]+)";
        private static readonly Regex FacebookUserNameRegex = new(FacebookUserNamePattern);
        
        public string Get(string userId)
        {
            Group group = FacebookUserNameRegex.Match(userId)?.Groups["userName"];

            return group.Success 
                ? group.Value.ToLower() 
                : null;
        }
    }
}