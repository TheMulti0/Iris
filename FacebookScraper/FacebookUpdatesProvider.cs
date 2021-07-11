using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using UpdatesScraper;

namespace FacebookScraper
{
    public class FacebookUpdatesProvider : IUpdatesProvider
    {
        private const string SharePrefixPattern = @"‏{0}‏\n‏\d{1,2}‏\s[\w\u0590-\u05FF]+\s·\n";
        private readonly PostsScraper _scraper;

        public FacebookUpdatesProvider(FacebookUpdatesProviderConfig config)
        {
            _scraper = new PostsScraper(config);
        }

        public async Task<IEnumerable<Update>> GetUpdatesAsync(User user)
        {
            var posts = await _scraper.GetPostsAsync(user);
                
            return posts.Select(ToUpdate(user));
        }

        private Func<Post, Update> ToUpdate(User user)
        {
            return post => new Update
            {
                Content = CleanText(post),
                Author = user,
                CreationDate = post.CreationDate,
                Url = post.Url,
                Media = GetMedia(post).ToList(),
                IsRepost = post.SharedPost != null,
                IsLive = post.IsLive
            };
        }

        private string CleanText(Post post)
        {
            if (post.SharedPost == null)
            {
                return post.EntireText;
            }
            
            string sharedPostText = GetSharedPostText(post);

            if (string.IsNullOrEmpty(post.PostTextOnly))
            {
                return sharedPostText;
            }

            return $"{post.PostTextOnly}\n---\n{sharedPostText}";
        }

        private string GetSharedPostText(Post post)
        {
            var regex = new Regex(SharePrefixPattern.Replace("{0}", post.SharedPost.Author.UserName));

            return regex.Replace(post.SharedPost.Text, string.Empty);
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