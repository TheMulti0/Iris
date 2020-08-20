using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace TelegramConsumer
{
    public class TelegramBot
    {
        private readonly ITelegramBotClientProvider _clientProvider;
        private readonly ILogger<TelegramBot> _logger;
        private readonly ILogger<MessageSender> _senderLogger;

        private ITelegramBotClient _client;
        private MessageSender _sender;
        private TelegramConfig _config;

        private CancellationTokenSource _sendCancellation;

        public TelegramBot(
            IConfigProvider configProvider,
            ITelegramBotClientProvider clientProvider,
            ILogger<TelegramBot> logger,
            ILogger<MessageSender> senderLogger)
        {
            _clientProvider = clientProvider;
            _logger = logger;
            _senderLogger = senderLogger;

            configProvider.Configs.SubscribeAsync(HandleConfig);
        }

        private Task HandleConfig(Result<TelegramConfig> result)
        {
            Task OnEmptyReceivedAsync()
            {
                _logger.LogInformation("Received empty config that will not be used");

                return Task.CompletedTask;
            }

            return result.DoAsync(
                OnConfigReceivedAsync,
                OnEmptyReceivedAsync);
        }

        private async Task OnConfigReceivedAsync(TelegramConfig config)
        {
            _logger.LogInformation(
                "Received new config {}, trying to create new TelegramBotClient with it",
                config);
            
            Optional<ITelegramBotClient> client = await CreateNewTelegramBotClient(config);
            
            if (client.HasValue)
            {
                CancelSendOperations();

                _client = client.Value;
                _sender = new MessageSender(_client, _senderLogger);
                _config = config;
            }
        }

        private async Task<Optional<ITelegramBotClient>> CreateNewTelegramBotClient(TelegramConfig config)
        {
            try
            {
                return Optional<ITelegramBotClient>.WithValue(
                    await _clientProvider.CreateAsync(config));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create TelegramBotClient with new config");
                return Optional<ITelegramBotClient>.Empty();
            }
        }

        private void CancelSendOperations()
        {
            _logger.LogInformation("Cancelling send operations");

            try
            {
                _sendCancellation?.Cancel();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to cancel send operations");
            }

            _sendCancellation = new CancellationTokenSource();
        }

        public async Task SendAsync(Update update)
        {
            if (_config == null)
            {
                _logger.LogError("Update request sent, but no config present. Leaving.");
                return;
            }

            _logger.LogInformation("Sending update {}", update);

            User user = _config.Users.First(u => u.UserName == update.AuthorId);

            UpdateMessage updateMessage = UpdateMessageFactory.Create(update, user);

            foreach (long chatId in user.ChatIds)
            {
                await _sender.SendAsync(updateMessage, chatId);
            }
        }
    }
}