using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Hosting;
using Scraper.Net;
using Scraper.RabbitMq.Client;

namespace MessagesManager
{
    internal class NewPostsConsumer : BackgroundService
    {
        public NewPostsConsumer(
            IScraperRabbitMqClient client,
            IConsumer<Update> consumer)
        {
            client.NewPosts
                .Select(ToUpdate)
                .SubscribeAsync(update => consumer.ConsumeAsync(update, CancellationToken.None));
        }

        private static Update ToUpdate(NewPost newPost)
        {
            (Post post, string platform) = newPost;

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

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }
}