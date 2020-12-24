using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TelegramSender.Tests
{
    internal class MockSenderFactory : ISenderFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public MockSenderFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Task<MessageSender> CreateAsync()
        {
            return
                Task.FromResult(
                    new MessageSender(
                        new MockTelegramBotClient(
                            _loggerFactory.CreateLogger<MockTelegramBotClient>()),
                        _loggerFactory));
        }
    }
}