using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Iris.Api;

namespace Iris.Facebook
{
    public class FacebookProvider : IUpdatesProvider
    {
        private readonly ILogger<FacebookProvider> _logger;
        private readonly int _pageCountPerUser;
        private readonly HttpClient _client;

        public FacebookProvider(
            ILogger<FacebookProvider> logger,
            ProviderConfig config)
        {
            _logger = logger;
            
            _pageCountPerUser = config.PageCountPerUser;
            _client = new HttpClient
            {
                BaseAddress = new Uri(config.ScraperUrl)
            };
            
            _logger.LogInformation("Completed construction");
        }

        public async Task<IEnumerable<Update>> GetUpdates(User user)
        {
            _logger.LogInformation($"GetUpdates requested with user {user.Id} (pageCount = {_pageCountPerUser})");
            
            Stream json = await GetFacebookPostsJson(user.Id, _pageCountPerUser);
            
            Post[] posts = await DeserializePosts(json);
            
            _logger.LogInformation($"Found {posts.Length} posts by {user.Id}");
            
            return posts
                .Select(post => post.ToUpdate(user));
        }

        private async Task<Stream> GetFacebookPostsJson(string userName, int pageCountPerUser)
        {
            HttpResponseMessage response = await _client.GetAsync(
                $"/facebook?name={userName}&pageCount={pageCountPerUser}");
            
            return await response.Content.ReadAsStreamAsync();
        }

        private static ValueTask<Post[]> DeserializePosts(Stream json)
        {
            return JsonSerializer
                .DeserializeAsync<Post[]>(json);
        }
    }
}