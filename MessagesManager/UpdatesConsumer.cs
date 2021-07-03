using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using SubscriptionsDb;
using UpdatesDb;

namespace MessagesManager
{
    internal class UpdatesConsumer : IConsumer<Update>
    {
        private readonly IProducer<Message> _producer;
        private readonly IChatSubscriptionsRepository _subscriptionsRepository;
        private readonly IUpdatesRepository _updatesRepository;
        private readonly VideoExtractor _videoExtractor;
        private readonly TweetScreenshotter _tweetScreenshotter;
        private readonly ILogger<UpdatesConsumer> _logger;

        public UpdatesConsumer(
            IProducer<Message> producer,
            IChatSubscriptionsRepository subscriptionsRepository,
            IUpdatesRepository updatesRepository,
            VideoExtractor videoExtractor,
            TweetScreenshotter tweetScreenshotter,
            ILogger<UpdatesConsumer> logger)
        {
            _producer = producer;
            _subscriptionsRepository = subscriptionsRepository;
            _updatesRepository = updatesRepository;
            _videoExtractor = videoExtractor;
            _tweetScreenshotter = tweetScreenshotter;
            _logger = logger;
        }

        public async Task ConsumeAsync(Update update, CancellationToken token)
        {
            _logger.LogInformation("Received {}", update);

            SubscriptionEntity entity = await _subscriptionsRepository.GetAsync(update.Author);
            List<UserChatSubscription> destinationChats = entity.Chats.ToList();

            Update newUpdate = await ModifiedUpdate(update, destinationChats, token);

            _producer.Send(
                new Message(
                    newUpdate,
                    destinationChats));

            //await _updatesRepository.AddOrUpdateAsync(new UpdateEntity(update));
        }

        private async Task<Update> ModifiedUpdate(
            Update update,
            IEnumerable<UserChatSubscription> destinationChats,
            CancellationToken token)
        {
            if (destinationChats.Any(subscription => subscription.SendScreenshotOnly))
            {
                string screenshotUrl = await _tweetScreenshotter.ScreenshotAsync(update.Url);

                var screenshot = new Photo(screenshotUrl);
                
                return update with { Media = new List<IMedia> { screenshot } };
            }

            List<IMedia> media = await WithExtractedVideos(update, token);

            return update with { Media = media };
        }

        private async Task<List<IMedia>> WithExtractedVideos(
            Update update,
            CancellationToken cancellationToken)
        {
            if (update.Author.Platform != Platform.Facebook)
            {
                return update.Media;
            }
            
            return await update.Media
                .ToAsyncEnumerable()
                .SelectAwait(m => ExtractVideo(update.Url, m))
                .ToListAsync(cancellationToken);
        }

        private async ValueTask<IMedia> ExtractVideo(
            string updateUrl, IMedia media)
        {
            if (media is not Video video)
            {
                return media;
            }
            
            try
            {
                return await GetExtractedVideo(updateUrl, video);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to extract video of url {}", updateUrl);
            }

            return media;
        }

        private async Task<IMedia> GetExtractedVideo(string updateUrl, Video old)
        {
            Video extracted = await _videoExtractor.ExtractVideo(updateUrl);
            
            if (extracted.ThumbnailUrl == null && old.ThumbnailUrl != null)
            {
                return extracted with { ThumbnailUrl = old.ThumbnailUrl };
            }
            
            return extracted;
        }
    }
}