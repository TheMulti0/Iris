using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Updates.Api;
using Updates.Configs;

namespace Updates.Facebook
{
    public class Facebook : IUpdatesProvider
    {
        private readonly int _pageCountPerUser;
        private readonly HttpClient _client;

        public Facebook(FacebookConfig config)
        {
            _pageCountPerUser = config.PageCountPerUser;
            _client = new HttpClient
            {
                BaseAddress = new Uri(config.ScraperUrl)
            };
        }

        public async Task<IEnumerable<Update>> GetUpdates(string userTokens)
        {
            var user = UserFactory.ToUser(userTokens);
            
            Stream json = await GetFacebookPostsJson(user.Id, _pageCountPerUser);

            Post[] posts = await DeserializePosts(json);
            
            return posts
                .Select(post => UpdateFactory.ToUpdate(post, user));
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