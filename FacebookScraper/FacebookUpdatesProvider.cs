using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UpdatesScraper;

namespace FacebookScraper
{
    public class FacebookUpdatesProvider : IUpdatesProvider
    {
        private readonly ILogger<FacebookUpdatesProvider> _logger;

        public FacebookUpdatesProvider(
            ILogger<FacebookUpdatesProvider> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Update>> GetUpdatesAsync(User user)
        {
            const string scriptName = "get_posts.py";
            
            try
            {
                // TODO make page count an optional configurable field
                string response = await ScriptExecutor.ExecutePython(
                    scriptName, token: default, user.UserId, 1);
                
                return JsonConvert.DeserializeObject<Post[]>(response)
                    .Select(ToUpdate(user));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse {} output for {}", scriptName, user);
            }

            return Enumerable.Empty<Update>();
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
                        "\n(?<link>[A-Z].+)"
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
            return post.Images.Select(
                url => new Photo(url));
        }
    }
}