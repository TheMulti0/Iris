using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.MassTransit.Common;
using Scraper.Net;
using SubscriptionsDb;

namespace TelegramSender
{
    public class NewPostConsumer : IConsumer<NewPost>
    {
        private readonly ITelegramMessageSender _telegram;
        private readonly VideoDownloader _videoDownloader;
        private readonly IChatSubscriptionsRepository _subscriptionsRepository;
        private readonly ILogger<NewPostConsumer> _logger;
        
        public NewPostConsumer(
            ITelegramMessageSender telegram,
            VideoDownloader videoDownloader,
            IChatSubscriptionsRepository subscriptionsRepository,
            ILoggerFactory loggerFactory)
        {
            _telegram = telegram;
            _videoDownloader = videoDownloader;
            _subscriptionsRepository = subscriptionsRepository;
            _logger = loggerFactory.CreateLogger<NewPostConsumer>();
        }

        public async Task Consume(ConsumeContext<NewPost> context)
        {
            CancellationToken ct = context.CancellationToken;
            NewPost newPost = context.Message;

            _logger.LogInformation("Received {}", newPost.Post.Url);

            if (newPost.Post.Type == PostType.Reply)
            {
                _logger.LogInformation("Dumping {}", newPost.Post.Url);
                return;
            }

            newPost = await WithDownloadedMediaAsync(newPost, ct);

            SubscriptionEntity entity = await _subscriptionsRepository.GetAsync(newPost.Post.AuthorId, newPost.Platform);
            List<UserChatSubscription> destinationChats = entity.Chats.ToList();

            var sendMessage = new SendMessage(newPost, destinationChats);

            await _telegram.ConsumeAsync(sendMessage, ct);
        }

        private async Task<NewPost> WithDownloadedMediaAsync(NewPost newPost, CancellationToken ct)
        {
            if (newPost.Platform != "facebook")
            {
                return newPost;
            }
            
            Post post = newPost.Post;
            IEnumerable<VideoItem> videos = post.MediaItems.OfType<VideoItem>().ToList();

            if (!videos.Any())
            {
                return newPost;
            }

            string url = videos.FirstOrDefault(video => video.UrlType == UrlType.WebpageUrl)?.Url ??
                         post.Url;
            
            string thumbnailUrl = videos
                .Select(i => i.ThumbnailUrl)
                .FirstOrDefault(u => u != null);

            var item = await DownloadVideoItem(url, thumbnailUrl, ct);

            IEnumerable<IMediaItem> newMediaItems = post.MediaItems
                .Where(i => i is not VideoItem)
                .Append(item);

            return newPost with { Post = post with { MediaItems = newMediaItems } };
        }

        private async Task<LocalVideoItem> DownloadVideoItem(string url, string thumbnailUrl, CancellationToken ct)
        {
            bool downloadThumbnail = thumbnailUrl == null;

            var item = await _videoDownloader.DownloadAsync(
                url,
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
