using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Iris.Api;
using Iris.Twitter.Factories;
using Microsoft.Extensions.Logging;

namespace Iris.Twitter
{
    public class TwitterProvider : IUpdatesProvider
    {
        private readonly ILogger<TwitterProvider> _logger;
        private readonly int _pageCountPerUser;
        private readonly HttpClient _client;

        public TwitterProvider(
            ILogger<TwitterProvider> logger,
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
            
            Stream json = await GetTwitterTweetsJson(user.Id, _pageCountPerUser);

            Tweet[] tweets = await DeserializeTweets(json);
            
            _logger.LogInformation($"Found {tweets.Length} tweets by {user.Id}");
            
            return tweets
                   .Select(tweet => tweet.ToUpdate(user));
        }

        private async Task<Stream> GetTwitterTweetsJson(string userName, int pageCountPerUser)
        {
            HttpResponseMessage response = await _client.GetAsync(
                $"/tweets?name={userName}&page_count={pageCountPerUser}");
            
            return await response.Content.ReadAsStreamAsync();
        }

        private static ValueTask<Tweet[]> DeserializeTweets(Stream json)
        {
            return JsonSerializer
                .DeserializeAsync<Tweet[]>(json);
        }
    }
}