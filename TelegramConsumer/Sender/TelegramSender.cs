using System;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramConsumer
{
    public class TelegramSender : ISender
    {
        private readonly ILogger<TelegramSender> _logger;

        private TelegramConfig _config;
        private ITelegramBotClient _client;

        private CancellationTokenSource _sendCancellation;

        public TelegramSender(
            ConfigsProvider configsProvider,
            ILogger<TelegramSender> logger)
        {
            _logger = logger;
            
            configsProvider.Configs
                .SubscribeAsync(HandleConfig);
            configsProvider.InitializeSubscriptions();
        }

        private Task HandleConfig(Result<TelegramConfig> result)
        {
            return result.DoAsync(
                OnConfigReceivedAsync,
                () =>
                {
                    _logger.LogInformation("Received empty config that will not be used");
                    
                    return Task.CompletedTask;
                });
        }

        private async Task OnConfigReceivedAsync(TelegramConfig config)
        {
            _logger.LogInformation(
                "Received new config {}, trying to create new TelegramBotClient with it",
                config);
            
            await ReplaceTelegramBotClient(config);
            
            CancelSendOperations();
        }

        private async Task ReplaceTelegramBotClient(TelegramConfig config)
        {
            try
            {
                _client = await CreateTelegramBotClient(config);
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

        private async Task<ITelegramBotClient> CreateTelegramBotClient(TelegramConfig config)
        {
            var client = new TelegramBotClient(config.AccessToken);

            User identity = await client.GetMeAsync();

            _logger.LogInformation(
                "Registered as {} {} (Username = {}, Id = {})",
                identity.FirstName,
                identity.LastName,
                identity.Username,
                identity.Id);

            return client;
        }

        public Task SendAsync(Update update)
        {
            _logger.LogInformation("Sending update {}", update);
            
            return Task.CompletedTask;
        }
    }
}