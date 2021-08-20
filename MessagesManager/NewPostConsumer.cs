using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MassTransit;
using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace MessagesManager
{
    public class NewPostConsumer : IConsumer<NewPost>
    {
        private readonly Extensions.IConsumer<Update> _consumer;

        public NewPostConsumer(Extensions.IConsumer<Update> consumer)
        {
            _consumer = consumer;
        }

        public async Task Consume(ConsumeContext<NewPost> context)
        {
            await _consumer.ConsumeAsync(ToUpdate(context.Message), context.CancellationToken);
        }

        private static Update ToUpdate(NewPost newPost)
        {
            var platform = newPost.Platform;
            var actualPlatform = Enum.Parse<Platform>(platform, true);
            
            var post = newPost.Post;

            return new Update
            {
                Author = new User(post.AuthorId, actualPlatform),
                Content = post.Content,
                Url = post.Url,
                CreationDate = post.CreationDate,
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
                case VideoItem v:
                    return new Video(v.Url, v.ThumbnailUrl, v.Duration, v.Width, v.Height);
                case AudioItem a:
                    return new Audio(a.Url, a.ThumbnailUrl, a.Duration, a.Title, a.Artist);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}