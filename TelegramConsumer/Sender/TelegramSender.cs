using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramConsumer
{
    public class TelegramSender : ISender
    {
        private readonly TelegramConfig _config;
        private readonly TelegramBotClient _client;
        private readonly ILogger<TelegramSender> _logger;

        public TelegramSender(
            TelegramConfig config,
            ILogger<TelegramSender> logger)
        {
            _config = config;
            _client = new TelegramBotClient(config.AccessToken);
            _logger = logger;

            var identity = _client.GetMeAsync().Result;

            _logger.LogInformation(
                "Registered as {} {} (Username = {}, Id = {})",
                identity.FirstName,
                identity.LastName,
                identity.Username,
                identity.Id);
        }

        public Task SendAsync(Update update)
        {
            _logger.LogInformation("Sending update {}", update);
            
            return Task.CompletedTask;
        }
    }
}