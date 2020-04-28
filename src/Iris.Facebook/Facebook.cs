using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Iris.Api;
using Newtonsoft.Json;

namespace Iris.Facebook
{
    public class Facebook : IUpdatesProvider
    {
        private readonly ILogger<Facebook> _logger;
        private readonly int _pageCountPerUser;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        public Facebook(
            ILogger<Facebook> logger,
            FacebookConfig config)
        {
            _logger = logger;
            
            _pageCountPerUser = config.PageCountPerUser;
            _client = new HttpClient
            {
                BaseAddress = new Uri(config.ScraperUrl)
            };
            
            _serializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            _logger.LogInformation("Completed construction");
        }

        public async Task<IEnumerable<Update>> GetUpdates(User user)
        {
            _logger.LogInformation($"GetUpdates requested with user {user.Id} (pageCount = {_pageCountPerUser})");
            
            string json = await GetFacebookPostsJson(user.Id, _pageCountPerUser);
            _logger.LogError("Posts from facebook are  \n \n \n \n" + json);
            
            Post[] posts = DeserializePosts(json);
            
            _logger.LogInformation($"Found {posts.Length} tweets by {user.Id}");
            
            return posts
                .Select(post => post.ToUpdate(user));
        }

        private async Task<string> GetFacebookPostsJson(string userName, int pageCountPerUser)
        {
            HttpResponseMessage response = await _client.GetAsync(
                $"/facebook?name={userName}&pageCount={pageCountPerUser}");
            
            return await response.Content.ReadAsStringAsync();
        }

        private Post[] DeserializePosts(string json)
        {
            return JsonSerializer.Deserialize<Post[]>(json, _serializerOptions);
        }
    }
}