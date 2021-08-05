using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using SubscriptionsDb;

namespace MessagesManager
{
    internal class UpdatesConsumer : IConsumer<Update>
    {
        private readonly IProducer<Message> _producer;
        private readonly IChatSubscriptionsRepository _subscriptionsRepository;
        private readonly ILogger<UpdatesConsumer> _logger;

        public UpdatesConsumer(
            IProducer<Message> producer,
            IChatSubscriptionsRepository subscriptionsRepository,
            ILogger<UpdatesConsumer> logger)
        {
            _producer = producer;
            _subscriptionsRepository = subscriptionsRepository;
            _logger = logger;
        }

        public async Task ConsumeAsync(Update update, CancellationToken token)
        {
            _logger.LogInformation("Received {}", update);

            SubscriptionEntity entity = await _subscriptionsRepository.GetAsync(update.Author);
            List<UserChatSubscription> destinationChats = entity.Chats.ToList();

            _producer.Send(new Message(update, destinationChats));    
        }
    }
}