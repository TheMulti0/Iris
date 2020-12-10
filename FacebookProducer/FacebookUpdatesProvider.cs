using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UpdatesProducer;

namespace FacebookProducer
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

        public async Task<IEnumerable<Update>> GetUpdatesAsync(string userId)
        {
            const string scriptName = "get_posts.py";
            
            try
            {
                // TODO make page count an optional configurable field
                string response = await ScriptExecutor.ExecutePython(
                    scriptName, userId, 1);
                
                return JsonConvert.DeserializeObject<Post[]>(response)
                    .Select(ToUpdate(userId));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse {} output", scriptName);
            }

            return Enumerable.Empty<Update>();
        }

        private Func<Post, Update> ToUpdate(string userId)
        {
            return post => new Update
            {
                Content = post.Text,
                AuthorId = userId,
                CreationDate = post.CreationDate,
                Url = post.PostUrl,
                Media = GetMedia(post).ToList(),
                Repost = post.Text == post.SharedText,
                Source = _config.Name
            };
        }

        private static IEnumerable<IMedia> GetMedia(Post post)
        {
            IEnumerable<Photo> photos = post.Images.Select(
                url => new Photo(url));

            if (post.VideoUrl == null)
            {
                return photos;
            }

            var video = new Video(
                post.VideoUrl,
                post.VideoThumbnailUrl,
                false);

            return photos.Concat(new IMedia[] { video });
        }
    }
}