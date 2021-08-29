﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scraper.Net;

namespace TelegramReceiver
{
    internal class UserValidator
    {
        private readonly IScraperService _service;
        private readonly Dictionary<string, IPlatformUserIdExtractor> _extractors;

        public UserValidator(
            IScraperService service,
            FacebookUserIdExtractor facebook,
            TwitterUserIdExtractor twitter)
        {
            _service = service;
            
            _extractors = new Dictionary<string, IPlatformUserIdExtractor>
            {
                {
                    "facebook",
                    facebook
                },
                {
                    "twitter",
                    twitter
                }
            };
        }
        
        public async Task<string> ValidateAsync(string userId, string platform)
        {
            var newUserId = GetUserId(userId, platform);

            var post = await ScrapeSinglePost(newUserId, platform);
            if (post == null)
            {
                throw new NullReferenceException(nameof(post));
            }
            
            string postAuthorId = post.AuthorId;
            
            return string.IsNullOrWhiteSpace(postAuthorId) 
                ? newUserId 
                : postAuthorId;
        }

        private string GetUserId(string userId, string platform)
        {
            if (!_extractors.ContainsKey(platform))
            {
                return userId;
            }
            
            return _extractors[platform].Get(userId) ?? userId;
        }
        
        private async Task<Post> ScrapeSinglePost(string userId, string platform)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            
            return await _service
                .GetPostsAsync(userId, platform, cts.Token)
                .FirstOrDefaultAsync(cts.Token);
        }
    }

}