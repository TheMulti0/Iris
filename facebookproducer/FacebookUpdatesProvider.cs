using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UpdatesProducer;

namespace FacebookProducer
{
    public class FacebookUpdatesProvider : IUpdatesProvider
    {
        private readonly ILogger<FacebookUpdatesProvider> _logger;

        public FacebookUpdatesProvider(ILogger<FacebookUpdatesProvider> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Update>> GetUpdatesAsync(string userId)
        {
            const string scriptName = "get_posts.py";
            
            try
            {
                string output = await ScriptExecutor.ExecutePython(
                    scriptName, userId, 1);
                
                return JsonConvert.DeserializeObject<Post[]>(output)
                    .Select(ToUpdate);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse {} output", scriptName);
            }

            return Enumerable.Empty<Update>();
        }

        private static Update ToUpdate(Post post)
        {
            return new()
            {
                Content = post.Text,
                AuthorId = post.AuthorId,
                CreationDate = post.CreationDate,
                Url = post.PostUrl,
                Media = GetMedia(post).ToList(),
                Repost = post.Text == post.SharedText
                // TODO redownload video
            };
        }

        private static IEnumerable<IMedia> GetMedia(Post post)
        {
            IEnumerable<Photo> photos = post.Images.Select(
                url => new Photo
                {
                    Url = url
                });

            if (post.VideoUrl == null)
            {
                return photos;
            }
            
            var video = new Video
            {
                Url = post.VideoUrl,
                ThumbnailUrl = post.VideoThumbnailUrl
            };

            return photos.Concat(new IMedia[] { video });
        }
    }
}