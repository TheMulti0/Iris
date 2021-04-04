using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UpdatesScraper;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FacebookScraper
{
    public class FacebookUpdatesProvider : IUpdatesProvider
    {
        private const string FacebookScriptName = "get_posts.py";
        private const string LinkRegex = "\n(?<link>[A-Z].+)";

        private readonly FacebookUpdatesProviderConfig _config;
        private readonly ILogger<FacebookUpdatesProvider> _logger;

        public FacebookUpdatesProvider(
            FacebookUpdatesProviderConfig config,
            ILogger<FacebookUpdatesProvider> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<IEnumerable<Update>> GetUpdatesAsync(User user)
        {
            try
            {
                string response = await GetFacebookResponse(user);

                Post[] posts = JsonConvert.DeserializeObject<Post[]>(response) ??
                               Array.Empty<Post>();

                if (!posts.Any())
                {
                    _logger.LogWarning("No results were received when scraping {}", user);
                }
                
                return posts.Select(ToUpdate(user)) ;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse {} output for {}", FacebookScriptName, user);
            }

            return Enumerable.Empty<Update>();
        }

        private Task<string> GetFacebookResponse(User user)
        {
            var parameters = new List<object>
            {
                user.UserId,
                _config.PageCount
            };
            
            if (_config.UserName != null && _config.Password != null)
            {
                parameters.Add(_config.UserName);
                parameters.Add(_config.Password);
            }
            
            return ScriptExecutor.ExecutePython(
                FacebookScriptName,
                token: default,
                parameters.ToArray());
        }

        private static Func<Post, Update> ToUpdate(User user)
        {
            return post => new Update
            {
                Content = ExtractText(post),
                Author = user,
                CreationDate = post.CreationDate,
                Url = post.PostUrl,
                Media = GetMedia(post).ToList(),
                IsRepost = post.Text == post.SharedText,
                IsLive = post.IsLive
            };
        }

        private static string ExtractText(Post post)
        {
            if (post.Link != null)
            {
                return post.Text.Replace(
                    new[]
                    {
                        LinkRegex
                    },
                    post.Link);
            }

            return post.Text;
        }

        private static IEnumerable<IMedia> GetMedia(Post post)
        {
            IEnumerable<Photo> photos = GetPhotos(post);

            if (post.VideoUrl == null)
            {
                return photos;
            }

            if (post.IsLive)
            {
                return new List<IMedia>();
            }
            
            var video = new Video(
                post.VideoUrl,
                post.VideoThumbnailUrl);

            return photos
                .Concat(new IMedia[] { video });
        }

        private static IEnumerable<Photo> GetPhotos(Post post)
        {
            return post.Images?.Select(url => new Photo(url)) ??
                   Enumerable.Empty<Photo>();
        }
    }
}