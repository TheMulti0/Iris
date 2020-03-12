using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Iris.Config;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Updates.Api;
using Updates.Configs;
using Updates.Twitter;
using Updates.Watcher;

namespace Iris
{
    internal class TelegramBot
    {
        private readonly ApplicationConfig _config;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<TelegramBot> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IUpdatesValidator _validator;

        public TelegramBot(
            ApplicationConfig config,
            ILoggerFactory loggerFactory,
            IUpdatesValidator validator)
        {
            _config = config;
            _loggerFactory = loggerFactory;
            
            _logger = _loggerFactory.CreateLogger<TelegramBot>();

            _client = new TelegramBotClient(config.TelegramBotConfig.Token);
            _client.StartReceiving();

            _validator = validator;

            _logger.LogInformation("Starting to receive Telegram events");

            RegisterProducers();

            _logger.LogInformation("Completed construction");
        }

        private void RegisterProducers()
        {
            foreach ((IUpdatesProvider provider, IProviderConfig config) in GetProviders())
            {
                try
                {
                    var usersWatcher = new UpdatesWatcher(
                        _loggerFactory.CreateLogger<UpdatesWatcher>(),
                        provider,
                        config);

                    usersWatcher.Updates
                        .Subscribe(OnProducerUpdate);

                    _logger.LogInformation($"Subscribed to updates of the producer `{provider.GetType() .Name}`");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception in RegisterProducers()");
                }
            }
        }

        private async void OnProducerUpdate(IUpdate update)
        {
            _logger.LogInformation($"Caught new update: Id: {update.Id, -15}, Author: {update.Author.Name, -15}, Created at: {update.CreatedAt}");
            foreach (long chatId in _config.TelegramBotConfig.UpdateChatsIds)
            {
                await SendMessage(update, chatId);
            }
        }

        private async Task SendMessage(IUpdate update, long chatId)
        {
            try
            {
                if (!_validator.WasUpdateSent(update.Id, chatId))
                {
                    _logger.LogInformation($"Update #{update.Id} was already sent to chat #{chatId}");
                    return;
                }

                await _client.SendTextMessageAsync(
                    chatId,
                    update.FormattedMessage,
                    ParseMode.Markdown);

                _logger.LogInformation(
                    $"Sent new update: Id: {update.Id, -15}, ChatId: {update.Author.Name, -15}, Executed at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                _logger.LogInformation(
                    e,
                    $"Failed to send update: Id: {update.Id, -15}, ChatId: {update.Author.Name, -15}, Executed at {DateTime.Now}");
            }
        }

        private Dictionary<IUpdatesProvider, IProviderConfig> GetProviders()
        {
            return new Dictionary<IUpdatesProvider, IProviderConfig>
            {
                {
                    new Twitter(
                        _loggerFactory.CreateLogger<Twitter>(),
                        _config.TwitterConfig),
                    
                    _config.TwitterConfig
                }
            };
        }
    }
}