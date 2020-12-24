using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UpdatesScraper
{
    public class UpdatesPollerService : BackgroundService
    {
        private readonly PollerConfig _config;
        private readonly IUpdatesPublisher _updatesPublisher;
        private readonly IUpdatesProvider _updatesProvider;
        private readonly IUserLatestUpdateTimesRepository _userLatestUpdateTimesRepository;
        private readonly ISentUpdatesRepository _sentUpdatesRepository;
        private readonly VideoExtractor _videoExtractor;
        private readonly ILogger<UpdatesPollerService> _logger;

        public UpdatesPollerService(
            PollerConfig config,
            IUpdatesPublisher updatesPublisher, 
            IUpdatesProvider updatesProvider,
            IUserLatestUpdateTimesRepository userLatestUpdateTimesRepository,
            ISentUpdatesRepository sentUpdatesRepository,
            VideoExtractor videoExtractor,
            ILogger<UpdatesPollerService> logger)
        {
            _config = config;
            _updatesPublisher = updatesPublisher;
            _updatesProvider = updatesProvider;
            _userLatestUpdateTimesRepository = userLatestUpdateTimesRepository;
            _sentUpdatesRepository = sentUpdatesRepository;
            _videoExtractor = videoExtractor;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                stoppingToken.ThrowIfCancellationRequested();

                _logger.LogInformation("Beginning to poll all users");
                
                await Poll(stoppingToken);
                
                _logger.LogInformation("Finished polling all users");
                _logger.LogInformation("Sleeping for {}", _config.Interval);

                await Task.Delay(_config.Interval, stoppingToken);
            }
        }

        private async Task Poll(CancellationToken cancellationToken)
        {
            foreach (string userId in _config.WatchedUserIds)
            {
                try
                {
                    await PollUser(userId, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to poll updates of {}", userId);
                }
            }
        }

        private async Task PollUser(string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Polling {}", userId);

            IEnumerable<Update> updates = await GetUpdates(userId, cancellationToken)
                .ToListAsync(cancellationToken);
            
            foreach (Update update in updates)
            {
                await SendUpdate(update);

                await Task.Delay(_config.SendDelay, cancellationToken);
            }

            if (updates.Any())
            {
                await _userLatestUpdateTimesRepository.AddOrUpdateAsync(userId, DateTime.Now);
            }
            else
            {
                _logger.LogInformation("No new updates found for {}", userId);
            }

        }

        private async Task SendUpdate(Update update)
        {
            _updatesPublisher.Send(update);

            if (_config.StoreSentUpdates)
            {
                await _sentUpdatesRepository.AddAsync(update.Url);
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
            var newUpdates = updates
                .Where(IsNew(userLatestUpdateTime))
                .OrderBy(update => update.CreationDate)
                .ToAsyncEnumerable();

            if (_config.StoreSentUpdates)
            {
                return newUpdates.WhereAwait(NotSent);
            }
            
            return newUpdates;
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