using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TelegramConsumer
{
    public class TelegramSender : ISender
    {
        private readonly ILogger<TelegramSender> _logger;

        public TelegramSender(ILogger<TelegramSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(Update update)
        {
            _logger.LogInformation("Sending update {}", update);

            return Task.CompletedTask;
        }
    }
}