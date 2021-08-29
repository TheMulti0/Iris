using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace TelegramSender
{
    public class MessageConsumer : IConsumer<SendMessage>
    {
        private readonly ITelegramMessageSender _telegram;
        private readonly VideoDownloader _videoDownloader;
        private readonly ILogger<MessageConsumer> _logger;
        
        public MessageConsumer(
            ITelegramMessageSender telegram,
            VideoDownloader videoDownloader,
            ILoggerFactory loggerFactory)
        {
            _telegram = telegram;
            _videoDownloader = videoDownloader;
            _logger = loggerFactory.CreateLogger<MessageConsumer>();
        }

        public async Task Consume(ConsumeContext<SendMessage> context)
        {
            SendMessage message = context.Message;
            CancellationToken ct = context.CancellationToken;

            _logger.LogInformation("Received {}", message);

            message = await WithDownloadedMediaAsync(message, ct);

            await _telegram.ConsumeAsync(message, ct);
        }

        private async Task<SendMessage> WithDownloadedMediaAsync(SendMessage message, CancellationToken ct)
        {
            NewPost newPost = message.NewPost;
            if (newPost.Platform != "facebook")
            {
                return message;
            }
            
            Post post = newPost.Post;
            IEnumerable<VideoItem> videos = post.MediaItems.OfType<VideoItem>().ToList();

            if (!videos.Any())
            {
                return message;
            }
            
            string thumbnailUrl = videos
                .Select(i => i.ThumbnailUrl)
                .FirstOrDefault(url => url != null);

            var item = await DownloadVideoItem(post, thumbnailUrl, ct);

            IEnumerable<IMediaItem> newMediaItems = post.MediaItems
                .Where(i => i is not VideoItem)
                .Append(item);

            return message with { NewPost = newPost with { Post = post with { MediaItems = newMediaItems } } };
        }

        private async Task<LocalVideoItem> DownloadVideoItem(Post post, string thumbnailUrl, CancellationToken ct)
        {
            bool downloadThumbnail = thumbnailUrl == null;

            var item = await _videoDownloader.DownloadAsync(
                post.Url,
                downloadThumbnail: downloadThumbnail,
                ct: ct);

            if (!downloadThumbnail)
            {
                return item with { ThumbnailUrl = thumbnailUrl, IsThumbnailLocal = false };
            }

            return item;
        }
    }
}
