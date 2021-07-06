using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

            IAsyncEnumerable<Message> messages = ToMessages(update, destinationChats, token);
            
            await foreach (Message message in messages.WithCancellation(token))
            {
                _producer.Send(message);    

                //await _updatesRepository.AddOrUpdateAsync(new UpdateEntity(message.Update));
            }
        }

        private async IAsyncEnumerable<Message> ToMessages(
            Update update,
            IReadOnlyCollection<UserChatSubscription> destinationChats,
            [EnumeratorCancellation] CancellationToken token)
        {
            List<UserChatSubscription> screenshotOnlySubscriptions = destinationChats
                .Where(subscription => subscription.SendScreenshotOnly)
                .ToList();

            bool hasSentScreenshot = false;

            if (screenshotOnlySubscriptions.Any())
            {
                Update screenshottedUpdate = await ScreenshotAsync(update);
                
                if (screenshottedUpdate != null)
                {
                    hasSentScreenshot = true;
                    
                    yield return new Message(
                        screenshottedUpdate,
                        screenshotOnlySubscriptions);
                }
            }

            List<UserChatSubscription> subscriptions = GetSubscriptions(destinationChats, screenshotOnlySubscriptions, hasSentScreenshot).ToList();

            if (subscriptions.Any())
            {
                Update extractedVideosUpdate = await ExtractVideosAsync(update, token);

                yield return new Message(
                    extractedVideosUpdate,
                    subscriptions);    
            }
        }

        private static IEnumerable<UserChatSubscription> GetSubscriptions(
            IEnumerable<UserChatSubscription> destinationChats,
            IEnumerable<UserChatSubscription> screenshotOnlySubscriptions,
            bool hasSentScreenshot)
        {
            var subscriptions = destinationChats.Where(subscription => !subscription.SendScreenshotOnly);

            return hasSentScreenshot 
                ? subscriptions 
                : subscriptions.Concat(screenshotOnlySubscriptions);
        }

        private async Task<Update> ScreenshotAsync(Update update)
        {
            try
            {
                return await _screenshotter.ScreenshotAsync(update);
            }
            catch (NotImplementedException)
            {
                return null;
            }
        }

        private async Task<Update> ExtractVideosAsync(
            Update update,
            CancellationToken token)
        {
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