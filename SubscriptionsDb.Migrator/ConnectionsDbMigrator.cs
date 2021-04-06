using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TelegramReceiver;

namespace SubscriptionsDb.Migrator
{
    public class ConnectionsDbMigrator : BackgroundService
    {
        private readonly TelegramBotClient _client;
        private readonly IConnectionsRepository _repository;

        public ConnectionsDbMigrator(TelegramBotClient client, IConnectionsRepository repository)
        {
            _client = client;
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                foreach (Connection connection in _repository.Get())
                {
                    if (connection.ChatId != 0)
                    {
                        continue;
                    }
                    var chat = await _client.GetChatAsync(connection.Chat, stoppingToken);

                    connection.ChatId = chat.Id;

                    await _repository.AddOrUpdateAsync(connection.User, connection);

                    Console.WriteLine($"Migrated connection {connection.User}");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}