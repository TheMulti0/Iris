using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace TelegramBot.Tests
{
    internal class MockTelegramBotClientProvider : ITelegramBotClientProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        public MockTelegramBotClientProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Task<ITelegramBotClient> CreateAsync(TelegramConfig config)
        {
            ITelegramBotClient client = new MockTelegramBotClient(
                _loggerFactory.CreateLogger<MockTelegramBotClient>());
            return Task.FromResult(client);
        }
    }
}