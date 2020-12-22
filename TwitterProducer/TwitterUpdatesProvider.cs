using System;
using System.Collections.Generic;
using System.Linq;
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
using UpdatesProducer;
using IMedia = Common.IMedia;

namespace TwitterProducer
{
    public class TwitterUpdatesProvider : IUpdatesProvider
    {
        private readonly ITwitterClient _twitterClient;
        private readonly UrlExpander _urlExpander;
        private readonly TwitterUpdatesProviderConfig _config;

        public TwitterUpdatesProvider(
            TwitterUpdatesProviderConfig config)
        {
            _twitterClient = new TwitterClient(
                config.ConsumerKey,
                config.ConsumerSecret,
                config.AccessToken,
                config.AccessTokenSecret);

            _urlExpander = new UrlExpander();

            _config = config;
        }

        public async Task<IEnumerable<Update>> GetUpdatesAsync(string userId)
        {
            var parameters = new GetUserTimelineParameters(userId)
            {
                PageSize = 10,
                TweetMode = TweetMode.Extended
            };
            ITweet[] tweets = await _twitterClient.Timelines.GetUserTimelineAsync(parameters);

            return tweets
                .Where(IsTweetPublishable(userId))
                .Select(ToUpdate(userId));
        }

        private static Func<ITweet, bool> IsTweetPublishable(string userId)
        {
            return tweet => tweet.InReplyToStatusId == null ||
                            tweet.InReplyToScreenName == userId; 
        }

        private Func<ITweet, Update> ToUpdate(string userId)
        {
            return tweet => new Update
            {
                Content = CleanText(tweet.IsRetweet 
                    ? tweet.RetweetedTweet.Text 
                    : tweet.FullText),
                AuthorId = userId,
                CreationDate = tweet.CreatedAt.DateTime,
                Url = tweet.Url,
                Media = GetMedia(tweet).ToList(),
                Repost = tweet.IsRetweet,
                Source = _config.Name
            };
        }

        private string CleanText(string text)
        {
            string withExpandedUrls = Regex.Replace(
                text,
                @"https://t.co/\S+",
                match => _urlExpander.ExpandAsync(match.Groups[0].Value).Result);

            return Replace(
                withExpandedUrls,
                new[]
                {
                    @"pic.twitter.com/\S+",
                    $@"(({TwitterConstants.TwitterBaseUrl}|{TwitterConstants.TwitterBaseUrlWww})/.+/status/\d+/(photo|video)/\d)"
                },
                string.Empty);
        }

        private static string Replace(
            string input,
            IEnumerable<string> patterns,
            string replacement)
        {
            string newestText = input;
            
            foreach (string pattern in patterns)
            {
                newestText = Regex.Replace(input, pattern, replacement);
            }

            return newestText;
        }
        
        private static IEnumerable<IMedia> GetMedia(ITweet tweet)
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