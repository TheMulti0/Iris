using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using UserDataLayer;

namespace MessagesManager
{
    public class UpdatesConsumer : IUpdatesConsumer
    {
        private readonly IMessagesProducer _producer;
        private readonly ISavedUsersRepository _repository;
        private readonly ILogger<IUpdatesConsumer> _logger;

        public UpdatesConsumer(
            IMessagesProducer producer,
            ISavedUsersRepository repository,
            ILogger<IUpdatesConsumer> logger)
        {
            _producer = producer;
            _repository = repository;
            _logger = logger;
        }

        public async Task OnUpdateAsync(Update update, CancellationToken token)
        {
            _logger.LogInformation("Received update {}", update);

            SavedUser savedUser = await _repository.GetAsync(update.Author);
            List<string> destinationChats = savedUser.Chats.Select(info => info.Chat).ToList();
            
            _producer.SendMessage(
                new Message(
                    update,
                    destinationChats));
        }
    }
}