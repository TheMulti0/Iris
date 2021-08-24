using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using SubscriptionsDb;

namespace MessagesManager
{
    public class UpdateConsumer
    {
        private readonly IChatSubscriptionsRepository _subscriptionsRepository;
        private readonly ILogger<UpdateConsumer> _logger;

        public UpdateConsumer(
            IChatSubscriptionsRepository subscriptionsRepository,
            ILogger<UpdateConsumer> logger)
        {
            _subscriptionsRepository = subscriptionsRepository;
            _logger = logger;
        }

        public async Task<Message> ConsumeAsync(Update update, CancellationToken token)
        {
            _logger.LogInformation("Received {}", update);

            SubscriptionEntity entity = await _subscriptionsRepository.GetAsync(update.Author);
            List<UserChatSubscription> destinationChats = entity.Chats.ToList();

            return new Message(update, destinationChats.ToList());    
        }
    }
}