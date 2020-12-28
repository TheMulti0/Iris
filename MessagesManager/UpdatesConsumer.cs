using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using UserDataLayer;

namespace MessagesManager
{
    public class UpdatesConsumer : IConsumer<Update>
    {
        private readonly IProducer<Message> _producer;
        private readonly ISavedUsersRepository _repository;
        private readonly ILogger<UpdatesConsumer> _logger;

        public UpdatesConsumer(
            IProducer<Message> producer,
            ISavedUsersRepository repository,
            ILogger<UpdatesConsumer> logger)
        {
            _producer = producer;
            _repository = repository;
            _logger = logger;
        }

        public async Task ConsumeAsync(Update update, CancellationToken token)
        {
            _logger.LogInformation("Received update {}", update);

            SavedUser savedUser = await _repository.GetAsync(update.Author);
            List<UserChatInfo> destinationChats = savedUser.Chats.ToList();
            
            _producer.Send(
                new Message(
                    update,
                    destinationChats));
        }
    }
}