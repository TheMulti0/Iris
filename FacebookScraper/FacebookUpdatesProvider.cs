using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UpdatesScraper;

namespace FacebookScraper
{
    public class FacebookUpdatesProvider : IUpdatesProvider
    {
        private readonly UpdatesProviderBaseConfig _config;
        private readonly ILogger<FacebookUpdatesProvider> _logger;

        public FacebookUpdatesProvider(
            UpdatesProviderBaseConfig config,
            ILogger<FacebookUpdatesProvider> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<IEnumerable<Update>> GetUpdatesAsync(User user)
        {
            const string scriptName = "get_posts.py";
            
            try
            {
                // TODO make page count an optional configurable field
                string response = await ScriptExecutor.ExecutePython(
                    scriptName, user.UserId, 1);
                
                return JsonConvert.DeserializeObject<Post[]>(response)
                    .Select(ToUpdate(user));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse {} output", scriptName);
            }

            return Enumerable.Empty<Update>();
        }

        private Func<Post, Update> ToUpdate(User user)
        {
            return post => new Update
            {
                Content = post.Text,
                Author = user,
                CreationDate = post.CreationDate,
                Url = post.PostUrl,
                Media = GetMedia(post).ToList(),
                Repost = post.Text == post.SharedText,
                Source = _config.Name
            };
        }

        private static IEnumerable<IMedia> GetMedia(Post post)
        {
            IEnumerable<Photo> photos = GetPhotos(post);

            if (post.VideoUrl == null)
            {
                return photos;
            }

            var video = new LowQualityVideo(
                post.VideoUrl,
                post.PostUrl,
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