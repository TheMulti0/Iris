using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;

namespace UpdatesScraper
{
    public class UpdatesScraper
    {
        private readonly ScraperConfig _config;
        private readonly IUpdatesProducer _updatesProducer;
        private readonly IUpdatesProvider _updatesProvider;
        private readonly IUserLatestUpdateTimesRepository _userLatestUpdateTimesRepository;
        private readonly ISentUpdatesRepository _sentUpdatesRepository;
        private readonly VideoExtractor _videoExtractor;
        private readonly ILogger<UpdatesScraper> _logger;

        public UpdatesScraper(
            ScraperConfig config,
            IUpdatesProducer updatesProducer, 
            IUpdatesProvider updatesProvider,
            IUserLatestUpdateTimesRepository userLatestUpdateTimesRepository,
            ISentUpdatesRepository sentUpdatesRepository,
            VideoExtractor videoExtractor,
            ILogger<UpdatesScraper> logger)
        {
            _config = config;
            _updatesProducer = updatesProducer;
            _updatesProvider = updatesProvider;
            _userLatestUpdateTimesRepository = userLatestUpdateTimesRepository;
            _sentUpdatesRepository = sentUpdatesRepository;
            _videoExtractor = videoExtractor;
            _logger = logger;
        }
        
        public async IAsyncEnumerable<Update> ScrapeUser(string userId, CancellationToken token)
        {
            _logger.LogInformation("Polling {}", userId);

            IAsyncEnumerable<Update> updates = GetUpdates(userId, token);
            
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
            string userId,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            IEnumerable<Update> updates = await _updatesProvider.GetUpdatesAsync(userId);
            List<Update> sortedUpdates = updates
                .Reverse()
                .OrderBy(update => update.CreationDate).ToList();

            UserLatestUpdateTime userLatestUpdateTime = await GetUserLatestUpdateTime(userId);

            ConfiguredCancelableAsyncEnumerable<Update> newUpdates = GetNewUpdates(sortedUpdates, userLatestUpdateTime)
                .WithCancellation(cancellationToken);

            await foreach (Update update in newUpdates)
            {
                yield return update.Media?.Any() == true
                    ? await WithExtractedVideo(update, cancellationToken) 
                    : update;
            }
        }

        private async ValueTask<Update> WithExtractedVideo(Update update, CancellationToken cancellationToken)
        {
            var newMedia = await update.Media.ToAsyncEnumerable()
                .SelectAwaitWithCancellation(ExtractVideo)
                .ToListAsync(cancellationToken);

            return update with { Media = newMedia };
        }

        private async ValueTask<IMedia> ExtractVideo(
            IMedia media, CancellationToken cancellationToken)
        {
            if (media is not LowQualityVideo video)
            {
                return media;
            }
            
            try
            {
                return await GetExtractedVideo(video);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to extract video of url {}", video.Url);
            }

            return media;
        }

        private async Task<IMedia> GetExtractedVideo(LowQualityVideo old)
        {
            Video extracted = await _videoExtractor.ExtractVideo(old.RequestUrl);
            
            if (extracted.ThumbnailUrl == null && old.ThumbnailUrl != null)
            {
                return extracted with { ThumbnailUrl = old.ThumbnailUrl };
            }
            
            return extracted;
        }

        private async Task<UserLatestUpdateTime> GetUserLatestUpdateTime(string userId)
        {
            var zero = new UserLatestUpdateTime
            {
                UserId = userId,
                LatestUpdateTime = DateTime.MinValue
            };
            
            return await _userLatestUpdateTimesRepository.GetAsync(userId) ?? zero;
        }

        private IAsyncEnumerable<Update> GetNewUpdates(
            IEnumerable<Update> updates,
            UserLatestUpdateTime userLatestUpdateTime)
        {
            IAsyncEnumerable<Update> newUpdates = updates
                .Where(IsNew(userLatestUpdateTime))
                .OrderBy(update => update.CreationDate)
                .ToAsyncEnumerable();

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
            return !await _sentUpdatesRepository.ExistsAsync(update.Url);
        }
    }
}