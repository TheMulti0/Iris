using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Common;

namespace TelegramReceiver
{
    public class TwitterUserIdExtractor : IPlatformUserIdExtractor
    {
        private const string TwitterUserNamePattern = @"(https?:\/\/(www\.)?(m.)?twitter.com\/)?@?(?<userName>[\w\d-_]+)";
        private static readonly Regex TwitterUserNameRegex = new(TwitterUserNamePattern);

        public string Get(string userId)
        {
            Group group = TwitterUserNameRegex.Match(userId)?.Groups["userName"];

            return group.Success 
                ? group.Value.ToLower() 
                : null;
        }
    }
}