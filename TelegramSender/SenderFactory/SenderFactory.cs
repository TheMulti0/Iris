using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramClient;

namespace TelegramSender
{
    public class SenderFactory : ISenderFactory
    {
        private readonly TelegramClientFactory _clientFactory;
        private readonly ILoggerFactory _factory;

        public SenderFactory(
            TelegramClientFactory clientFactory,
            ILoggerFactory factory)
        {
            _clientFactory = clientFactory;
            _factory = factory;
        }

        public async Task<MessageSender> CreateAsync()
        {
            ITelegramClient telegramClient = await _clientFactory.CreateAsync();
            
            return new MessageSender(telegramClient, _factory);
        }
    }
}