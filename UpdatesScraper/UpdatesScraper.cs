using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Scraper.Net;

namespace UpdatesScraper
{
    public class UpdatesScraper
    {
        private readonly ScraperConfig _config;
        private readonly IScraperService _scraperService;
        private readonly IUserLatestUpdateTimesRepository _userLatestUpdateTimesRepository;
        private readonly ISentUpdatesRepository _sentUpdatesRepository;
        private readonly ILogger<UpdatesScraper> _logger;

        public UpdatesScraper(
            ScraperConfig config,
            IScraperService scraperService,
            IUserLatestUpdateTimesRepository userLatestUpdateTimesRepository,
            ISentUpdatesRepository sentUpdatesRepository,
            ILogger<UpdatesScraper> logger)
        {
            _config = config;
            _scraperService = scraperService;
            _userLatestUpdateTimesRepository = userLatestUpdateTimesRepository;
            _sentUpdatesRepository = sentUpdatesRepository;
            _logger = logger;
        }
        
        public async IAsyncEnumerable<Update> ScrapeUser(
            User user,
            [EnumeratorCancellation] CancellationToken token)
        {
            _logger.LogInformation("Polling {}", user);

            IAsyncEnumerable<Update> updates = GetUpdates(user, token);
            
            await foreach (Update update in updates.WithCancellation(token))
            {
                if (_config.StoreSentUpdates)
                {
                    await _sentUpdatesRepository.AddAsync(update.Url);
                }
                
                yield return update;
            }
        }

        private async IAsyncEnumerable<Update> GetUpdates(
            User user,
            [EnumeratorCancellation] CancellationToken ct)
        {
            string platform = GetPlatform(user);
            
            var posts = _scraperService.GetPostsAsync(user.UserId, platform, ct);
            var updates = posts.Select(update => ToUpdate(update, platform));
            
            UserLatestUpdateTime userLatestUpdateTime = await GetUserLatestUpdateTime(user);

            var newPosts = GetNewUpdates(updates, userLatestUpdateTime);

            await foreach (Update update in newPosts.WithCancellation(ct))
            {
                yield return update;
            }
        }

        private static Update ToUpdate(Post post, string platform)
        {
            var author = new User(
                post.AuthorId,
                Enum.Parse<Platform>(platform, ignoreCase: true));
            
            return new Update
            {
                Author = author,
                Content = post.Content,
                CreationDate = post.CreationDate,
                Url = post.Url,
                IsLive = post.IsLivestream,
                IsReply = post.Type == PostType.Reply,
                IsRepost = post.Type == PostType.Repost,
                Media = post.MediaItems.Select(ToMedia).ToList()
            };
        }

        private static IMedia ToMedia(IMediaItem item)
        {
            switch (item)
            {
                case PhotoItem p:
                    return new Photo(p.Url);
                case AudioItem a:
                    return new Audio(a.Url, a.ThumbnailUrl, a.Duration, a.Title, a.Artist);
                case VideoItem v:
                    return new Video(v.Url, v.ThumbnailUrl, v.Duration, v.Width, v.Height);
            }

            return null;
        }

        private static string GetPlatform(User user)
        {
            switch (user.Platform)
            {
                case Platform.Facebook:
                    return "facebook";
                case Platform.Feeds:
                    return "feeds";
                case Platform.Twitter:
                    return "twitter";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<UserLatestUpdateTime> GetUserLatestUpdateTime(User user)
        {
            var zero = new UserLatestUpdateTime
            {
                User = user,
                LatestUpdateTime = DateTime.MinValue
            };
            
            return await _userLatestUpdateTimesRepository.GetAsync(user) ?? zero;
        }

        private IAsyncEnumerable<Update> GetNewUpdates(
            IAsyncEnumerable<Update> updates,
            UserLatestUpdateTime userLatestUpdateTime)
        {
            IAsyncEnumerable<Update> newUpdates = updates
                .Where(IsNew(userLatestUpdateTime))
                .OrderBy(update => update.CreationDate);

            return _config.StoreSentUpdates 
                ? newUpdates.WhereAwait(NotSent) 
                : newUpdates;
        }

        private static Func<Update, bool> IsNew(UserLatestUpdateTime userLatestUpdateTime)
        {
            return update => update.CreationDate != null &&
                             update.CreationDate > userLatestUpdateTime.LatestUpdateTime;
        }

        private async ValueTask<bool> NotSent(Update update)
        {
            return update.Url != null && 
                   !await _sentUpdatesRepository.ExistsAsync(update.Url);
        }
    }
}