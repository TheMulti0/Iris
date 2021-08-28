using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.RabbitMq.Common;
using SubscriptionsDb;

namespace MessagesManager
{
    public class NewPostConsumer : IConsumer<NewPost>
    {
        private readonly IChatSubscriptionsRepository _subscriptionsRepository;
        private readonly ILogger<NewPostConsumer> _logger;

        public NewPostConsumer(
            IChatSubscriptionsRepository subscriptionsRepository,
            ILogger<NewPostConsumer> logger)
        {
            _subscriptionsRepository = subscriptionsRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<NewPost> context)
        {
            NewPost newPost = context.Message;
            _logger.LogInformation("Received {}", newPost.Post.Url);

            SubscriptionEntity entity = await _subscriptionsRepository.GetAsync(newPost.Post.AuthorId, newPost.Platform);
            List<UserChatSubscription> destinationChats = entity.Chats.ToList();

            var message = new SendMessage(newPost, destinationChats.ToList());

            await context.Publish(message);
        }
    }
}