using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Extensions;
using Optional;
using Optional.Unsafe;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using Tweetinvi.Models.Entities.ExtendedEntities;
using Tweetinvi.Parameters;
using UpdatesScraper;
using IMedia = Common.IMedia;

namespace TwitterScraper
{
    public class TwitterUpdatesProvider : IUpdatesProvider
    {
        internal ITwitterClient TwitterClient { get; }
        
        private readonly UrlExpander _urlExpander;

        public TwitterUpdatesProvider(
            TwitterUpdatesProviderConfig config)
        {
            TwitterClient = new TwitterClient(
                config.ConsumerKey,
                config.ConsumerSecret);

            TwitterClient.Auth.InitializeClientBearerTokenAsync().Wait();

            _urlExpander = new UrlExpander();
        }

        public Task<IEnumerable<Update>> GetUpdatesAsync(User user)
        {
            return GetUpdatesAsync(user, false);
        }
        
        public Task<IEnumerable<Update>> GetAllUpdatesAsync(User user)
        {
            return GetUpdatesAsync(user, true);
        }
        
        public async Task<IEnumerable<Update>> GetUpdatesAsync(
            User user, bool includeAll)
        {
            var parameters = new GetUserTimelineParameters(user.UserId)
            {
                PageSize = 10,
                TweetMode = TweetMode.Extended
            };
            ITweet[] tweets = await TwitterClient.Timelines.GetUserTimelineAsync(parameters);

            IEnumerable<ITweet> updates = tweets;

            if (!includeAll)
            {
                updates = updates.Where(IsTweetPublishable(user.UserId));
            }
                
            return updates.Select(ToUpdate(user));
        }

        private static Func<ITweet, bool> IsTweetPublishable(string userId)
        {
            return tweet => tweet.InReplyToStatusId == null ||
                            tweet.InReplyToScreenName == userId; 
        }

        private Func<ITweet, Update> ToUpdate(User user)
        {
            return tweet => new Update
            {
                Content = CleanText(tweet.IsRetweet 
                    ? tweet.RetweetedTweet.Text 
                    : tweet.FullText),
                Author = user,
                CreationDate = tweet.CreatedAt.DateTime,
                Url = tweet.Url,
                Media = GetMedia(tweet).ToList(),
                IsRepost = tweet.IsRetweet,
                IsReply = tweet.InReplyToScreenName != null
            };
        }

        internal string CleanText(string text)
        {
            string withExpandedUrls = Regex.Replace(
                text,
                @"https://t.co/\S+",
                match => _urlExpander.ExpandAsync(match.Groups[0].Value).Result);

            return withExpandedUrls.Replace(
                new[]
                {
                    @"(https://)?pic.twitter.com/\S+",
                    $@"(({TwitterConstants.TwitterBaseUrl}|{TwitterConstants.TwitterBaseUrlWww})/.+/status/\d+/(photo|video)/\d)"
                },
                string.Empty);
        }
        
        internal static IEnumerable<IMedia> GetMedia(ITweet tweet)
        {
            List<IMediaEntity> medias = tweet.ExtendedTweet?.ExtendedEntities?.Medias 
                                        ?? tweet.Media 
                                        ?? new List<IMediaEntity>();

            foreach (IMediaEntity media in medias)
            {
                string url = media.MediaURLHttps ?? media.MediaURL;

                if (media.MediaType == "photo")
                {
                    yield return new Photo(url);
                }
                else
                {
                    Option<IMedia> video = GetVideo(media, url);
                    
                    if (video.HasValue)
                    {
                        yield return video.ValueOrDefault();
                    }
                }
            }
        }

        private static Option<IMedia> GetVideo(IMediaEntity media, string thumbnailUrl)
        {
            IVideoInformationEntity videoInfo = media.VideoDetails;
            IVideoEntityVariant[] variants = videoInfo.Variants;

            IVideoEntityVariant bestVideo = variants.OrderByDescending(variant => variant.Bitrate)
                .FirstOrDefault();

            Dictionary<string, IMediaEntitySize> sizes = media.Sizes;
            IMediaEntitySize size = sizes.GetValueOrDefault("large") 
                                    ?? sizes.GetValueOrDefault("medium") 
                                    ?? sizes.GetValueOrDefault("small");

            if (bestVideo != null)
            {
                return Option.Some<IMedia>(
                    new Video(
                        bestVideo.URL,
                        thumbnailUrl,
                        TimeSpan.FromMilliseconds(videoInfo.DurationInMilliseconds),
                        size?.Width,
                        size?.Height));
            }
            
            return Option.None<IMedia>();
        }
    }
}