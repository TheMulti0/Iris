﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using TwitterScraper;

namespace TelegramReceiver
{
    public class TwitterValidator : IPlatformValidator
    {
        private const string TwitterUserNamePattern = @"(https?:\/\/(www\.)?(m.)?twitter.com\/)?@?(?<userName>[\w\d-_]+)";
        private static readonly Regex TwitterUserNameRegex = new(TwitterUserNamePattern);
        
        private readonly TwitterUpdatesProvider _twitter;

        public TwitterValidator(TwitterUpdatesProvider twitter)
        {
            _twitter = twitter;
        }

        public async Task<User> ValidateAsync(string userId)
        {
            Group group = TwitterUserNameRegex.Match(userId)?.Groups["userName"];

            if (!group.Success)
            {
                return null;
            }

            User newUser = new User(group.Value.ToLower(), Platform.Twitter);
            
            IEnumerable<Update> updates = await GetUpdatesAsync(newUser);

            if (updates == null || !updates.Any())
            {
                return null;
            }

            return newUser;
        }

        private async Task<IEnumerable<Update>> GetUpdatesAsync(User newUser)
        {
            try
            {
                return await _twitter.GetAllUpdatesAsync(newUser);
            }
            catch
            {
                return null;
            }
        }
    }
}