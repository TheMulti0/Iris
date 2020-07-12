using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace TelegramConsumer
{
    public class TelegramSender : ISender
    {
        private readonly ITelegramBotClientProvider _clientProvider;
        private readonly MessageSender _sender;
        private readonly ILogger<TelegramSender> _logger;

        private ITelegramBotClient _client;
        private TelegramConfig _config;

        private CancellationTokenSource _sendCancellation;

        public TelegramSender(
            ConfigsProvider configsProvider,
            ITelegramBotClientProvider clientProvider,
            MessageSender sender,
            ILogger<TelegramSender> logger)
        {
            _clientProvider = clientProvider;
            _sender = sender;
            _logger = logger;
            
            configsProvider.Configs
            
                .SubscribeAsync(HandleConfig);
            configsProvider.InitializeSubscriptions();
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
            
            await ReplaceTelegramBotClient(config);

            _config = config;
            
            CancelSendOperations();
        }

        private async Task ReplaceTelegramBotClient(TelegramConfig config)
        {
            try
            {
                _client = await _clientProvider.CreateAsync(config);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Failed to create TelegramBotClient with new config");
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
            }

            _logger.LogInformation("Sending update {}", update);

            var user = _config.Users.FirstOrDefault(user => user.UserName == update.AuthorId);
            var updateMessage = UpdateMessageFactory.Create(update, user);

            foreach (var chatId in user.ChatIds)
            {
                await _sender.SendAsync(_client, updateMessage, chatId);
            }
        }
    }
}