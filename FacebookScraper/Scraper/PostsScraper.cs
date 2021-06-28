using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UpdatesScraper;

namespace FacebookScraper
{
    public class PostsScraper
    {
        private const string FacebookScriptName = "get_posts.py";

        private readonly FacebookUpdatesProviderConfig _config;
        private readonly ILogger<PostsScraper> _logger;

        public PostsScraper(
            FacebookUpdatesProviderConfig config,
            ILogger<PostsScraper> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(User user)
        {
            try
            {
                string response = await GetFacebookResponse(user);

                PostRaw[] posts = JsonConvert.DeserializeObject<PostRaw[]>(response) ??
                                  Array.Empty<PostRaw>();
                
                if (!posts.Any())
                {
                    _logger.LogWarning("No results were received when scraping {} {}", user, response);
                }

                return posts.Select(raw => raw.ToPost());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse {} output for {}", FacebookScriptName, user);
            }

            return Enumerable.Empty<Post>();
        }

        private Task<string> GetFacebookResponse(User user)
        {
            var parameters = new List<object>
            {
                user.UserId,
                _config.PageCount,
                string.Join(',', _config.Proxies)
            };

            return ScriptExecutor.ExecutePython(
                FacebookScriptName,
                token: default,
                parameters.ToArray());
        }
    }
}