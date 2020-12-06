using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json;
using UpdatesProducer;

namespace FacebookProducer
{
    public class FacebookUpdatesProvider : IUpdatesProvider
    {
        public async Task<IEnumerable<Update>> GetUpdatesAsync(string userId)
        {
            var output = await ScriptExecutor.Execute(
                "python",
                "get_posts.py",
                new List<string>
                {
                    "Netanyahu",
                    "1"
                });

            return JsonConvert.DeserializeObject<Post[]>(output)
                .Select(ToUpdate);
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