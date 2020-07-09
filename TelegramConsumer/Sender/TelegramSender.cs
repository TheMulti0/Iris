using System;
using System.Reactive.Linq;
using System.Text.Json;
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
        private TelegramBotClient _client;

        private CancellationTokenSource _sendCancellation;

        public TelegramSender(
            Consumer<string, string> consumer,
            ILogger<TelegramSender> logger)
        {
            _logger = logger;
            
            consumer.Messages
                .Where(ConfigBelongsToTelegram)
                .Select(DeserializeConfig)
                .SubscribeAsync(HandleConfig);
        }

        private static bool ConfigBelongsToTelegram(Result<Message<string, string>> result)
        {
            return result.Value?.Value.ValueEqualsTo("Telegram") ?? false;
        }

        private static Result<TelegramConfig> DeserializeConfig(Result<Message<string, string>> result)
        {
            return result.Map(message => JsonSerializer.Deserialize<TelegramConfig>(message.Value.Value));
        }

        private Task HandleConfig(Result<TelegramConfig> result)
        {
            return result.DoAsync(OnConfigReceivedAsync);
        }

        private async Task OnConfigReceivedAsync(TelegramConfig config)
        {
            _logger.LogInformation("Received new config. Cancelling send operations");
            
            _sendCancellation?.Cancel();
            _sendCancellation = new CancellationTokenSource();
            
            _config = config;
            _client = new TelegramBotClient(config.AccessToken);

            User identity = await _client.GetMeAsync();

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