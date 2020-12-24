using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramSender
{
    public class SenderFactory : ISenderFactory
    {
        private readonly TelegramConfig _config;
        private readonly ILoggerFactory _factory;
        private readonly ILogger<SenderFactory> _logger;

        public SenderFactory(
            TelegramConfig config,
            ILoggerFactory factory)
        {
            _config = config;
            _factory = factory;
            _logger = factory.CreateLogger<SenderFactory>();
        }

        public async Task<MessageSender> CreateAsync()
        {
            var client = new TelegramBotClient(_config.AccessToken);

            User identity = await client.GetMeAsync();

            _logger.LogInformation(
                "Logged into Telegram as {} {} (Username = {}, Id = {})",
                identity.FirstName,
                identity.LastName,
                identity.Username,
                identity.Id);

            return new MessageSender(client, _factory);
        }
    }
}