using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using UpdatesScraper;

namespace FacebookScraper
{
    public class FacebookUpdatesProvider : IUpdatesProvider
    {
        private readonly PostsScraper _scraper;

        public FacebookUpdatesProvider(
            FacebookUpdatesProviderConfig config,
            ILoggerFactory loggerFactory)
        {
            _scraper = new PostsScraper(
                config,
                loggerFactory.CreateLogger<PostsScraper>());
        }

        public async Task<IEnumerable<Update>> GetUpdatesAsync(User user)
        {
            var posts = await _scraper.GetPostsAsync(user);
                
            return posts.Select(ToUpdate(user));
        }

        private static Func<Post, Update> ToUpdate(User user)
        {
            return post => new Update
            {
                Content = post.EntireText,
                Author = user,
                CreationDate = post.CreationDate,
                Url = post.Url,
                Media = GetMedia(post).ToList(),
                IsRepost = post.SharedPost != null,
                IsLive = post.IsLive
            };
        }

        private static IEnumerable<IMedia> GetMedia(Post post)
        {
            IEnumerable<Photo> photos = GetPhotos(post);

            if (post.Video == null)
            {
                return photos;
            }

            if (post.IsLive)
            {
                return new List<IMedia>();
            }
            
            var video = new Common.Video(
                post.Video.Url,
                post.Video.ThumbnailUrl,
                post.Video.Duration,
                post.Video.Width,
                post.Video.Height);

            return photos
                .Concat(new IMedia[] { video });
        }

        private static IEnumerable<Photo> GetPhotos(Post post)
        {
            return post.Images?.Select(img => new Photo(img.Url)) ??
                   Enumerable.Empty<Photo>();
        }
    }
}