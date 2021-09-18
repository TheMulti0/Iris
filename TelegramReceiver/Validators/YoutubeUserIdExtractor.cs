using System.Text.RegularExpressions;

namespace TelegramReceiver
{
    public class YoutubeUserIdExtractor : IPlatformUserIdExtractor
    {
        private const string YoutubeUserNamePattern = @"(https:\/\/www\.youtube\.com\/channel\/)?(?<userName>[\w-]+)";
        private static readonly Regex YoutubeUserNameRegex = new(YoutubeUserNamePattern);
        
        public string Get(string userId)
        {
            Group group = YoutubeUserNameRegex.Match(userId)?.Groups["userName"];

            return group.Success 
                ? group.Value
                : null;
        }
    }
}