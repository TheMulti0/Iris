using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class TelegramBotClientProvider : ITelegramBotClientProvider
    {
        private readonly ILogger<TelegramBotClientProvider> _logger;

        public TelegramBotClientProvider(ILogger<TelegramBotClientProvider> logger)
        {
            _logger = logger;
        }

        public async Task<ITelegramBotClient> CreateAsync(TelegramConfig config)
        {
            var client = new TelegramBotClient(config.AccessToken);

            Telegram.Bot.Types.User identity = await client.GetMeAsync();

            _logger.LogInformation(
                "Registered as {} {} (Username = {}, Id = {})",
                identity.FirstName,
                identity.LastName,
                identity.Username,
                identity.Id);

            return client;
        }
    }
}