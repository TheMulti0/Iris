using System;
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
        private readonly string[] _platformWithHighQualityMedia =
        {
            "twitter",
            "feeds"
        };
        
        private readonly ITelegramMessageSender _telegram;
        private readonly HighQualityVideoExtractor _hq;
        private readonly IChatSubscriptionsRepository _subscriptionsRepository;
        private readonly ILogger<NewPostConsumer> _logger;
        
        public NewPostConsumer(
            ITelegramMessageSender telegram,
            HighQualityVideoExtractor hq,
            IChatSubscriptionsRepository subscriptionsRepository,
            ILoggerFactory loggerFactory)
        {
            _telegram = telegram;
            _hq = hq;
            _subscriptionsRepository = subscriptionsRepository;
            _logger = loggerFactory.CreateLogger<NewPostConsumer>();
        }

        public async Task Consume(ConsumeContext<NewPost> context)
        {
            CancellationToken ct = context.CancellationToken;
            NewPost newPost = context.Message;
            
            await foreach (NewPost post in ProcessAsync(newPost, ct))
            {
                await ConsumeSingle(post, ct);
            }
        }

        private async Task ConsumeSingle(NewPost newPost, CancellationToken ct)
        {
            _logger.LogInformation("Received {}", newPost.Post.Url);

            if (newPost.Post.Type == PostType.Reply)
            {
                _logger.LogInformation("Dumping {}", newPost.Post.Url);
                return;
            }

            try
            {
                newPost = await ProcessSinglePostAsync(newPost, ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to process media");
            }

            SubscriptionEntity entity = await _subscriptionsRepository.GetAsync(newPost.Post.AuthorId, newPost.Platform);
            List<UserChatSubscription> destinationChats = entity.Chats.ToList();

            var sendMessage = new SendMessage(newPost, destinationChats);

            await _telegram.ConsumeAsync(sendMessage, ct);
        }

        private IAsyncEnumerable<NewPost> ProcessAsync(NewPost newPost, CancellationToken ct)
        {
            if (_platformWithHighQualityMedia.Contains(newPost.Platform))
            {
                return new[] { newPost }.ToAsyncEnumerable();
            }

            return GetAll(newPost.Post)
                .ToAsyncEnumerable()
                .Select(post => newPost with { Post = post });
        }

        private static IEnumerable<Post> GetAll(Post post)
        {
            Post currentPost = post;
            while (true)
            {
                yield return currentPost;

                Post reply = currentPost.ReplyPost;
                if (reply == null)
                {
                    break;
                }
                
                currentPost = reply;
            }
        }

        private async Task<NewPost> ProcessSinglePostAsync(NewPost post, CancellationToken ct)
        {
            IEnumerable<IMediaItem> items = await GetMediaItems(post.Post, ct);

            return post with { Post = post.Post with { MediaItems = items } };
        }
        

        private async Task<IEnumerable<IMediaItem>> GetMediaItems(
            Post post,
            CancellationToken ct)
        {
            IEnumerable<VideoItem> videos = post.MediaItems.OfType<VideoItem>().ToList();

            if (!videos.Any())
            {
                return post.MediaItems;
            }

            if (post.IsLivestream)
            {
                return post.MediaItems.Except(videos);
            }

            string url = videos
                .FirstOrDefault(video => video.UrlType == UrlType.WebpageUrl)?.Url ?? post.Url;

            string thumbnailUrl = videos
                .Select(i => i.ThumbnailUrl)
                .FirstOrDefault(u => u != null);

            var item = await DownloadVideoItem(url, thumbnailUrl, ct);

            IEnumerable<IMediaItem> newMediaItems = post.MediaItems
                .Where(i => i is not VideoItem)
                .Append(item);

            return newMediaItems;
        }

        private async Task<IMediaItem> DownloadVideoItem(string url, string thumbnailUrl, CancellationToken ct)
        {
            bool downloadThumbnail = thumbnailUrl == null;

            var item = await _hq.ExtractAsync(
                url,
                downloadThumbnail: downloadThumbnail,
                ct: ct);

            if (!downloadThumbnail && item is LocalVideoItem l)
            {
                return l with { ThumbnailUrl = thumbnailUrl, IsThumbnailLocal = false };
            }

            return item;
        }
    }
}
