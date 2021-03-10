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
        private readonly Screenshotter _screenshotter;
        private readonly ILogger<UpdatesConsumer> _logger;

        public UpdatesConsumer(
            IProducer<Message> producer,
            IChatSubscriptionsRepository subscriptionsRepository,
            IUpdatesRepository updatesRepository,
            VideoExtractor videoExtractor,
            Screenshotter screenshotter,
            ILogger<UpdatesConsumer> logger)
        {
            _producer = producer;
            _subscriptionsRepository = subscriptionsRepository;
            _updatesRepository = updatesRepository;
            _videoExtractor = videoExtractor;
            _screenshotter = screenshotter;
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

            await _updatesRepository.AddOrUpdateAsync(new UpdateEntity(update));
        }

        private async Task<Update> ModifiedUpdate(
            Update update,
            IEnumerable<UserChatSubscription> destinationChats,
            CancellationToken token)
        {
            List<IMedia> media = await WithExtractedVideos(update, token);

            if (!destinationChats.Any(subscription => subscription.SendScreenshotOnly))
            {
                return update with { Media = media };
            }
            
            byte[] screenshot = await _screenshotter.ScreenshotAsync(update);

            return update with { Media = media, Screenshot = screenshot };
        }

        private async Task<List<IMedia>> WithExtractedVideos(
            Update update,
            CancellationToken cancellationToken)
        {
            if (update.Author.Platform == Platform.Twitter)
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
                _logger.LogError(e, "Failed to extract video of url {}", video.Url);
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