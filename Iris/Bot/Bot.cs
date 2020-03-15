using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Iris.Config;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Updates.Api;
using Updates.Configs;
using Updates.Twitter;
using Updates.Watcher;
using Update = Updates.Api.Update;

namespace Iris.Bot
{
    internal class Bot
    {
        private readonly ApplicationConfig _config;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<Bot> _logger;
        private readonly Sender _sender;        
        private readonly IUpdatesValidator _validator;

        public Bot(
            ApplicationConfig config,
            ILoggerFactory loggerFactory,
            IUpdatesValidator validator)
        {
            _config = config;
            _loggerFactory = loggerFactory;
            
            _logger = _loggerFactory.CreateLogger<Bot>();

            var client = new TelegramBotClient(config.TelegramBotConfig.Token);
            _sender = new Sender(
                client,
                loggerFactory.CreateLogger<Sender>());
            
            client.StartReceiving();
            client.OnUpdate += (sender, args) =>
            {

            };
            
            _validator = validator;

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

        private async void OnProducerUpdate(Update update)
        {
            _logger.LogInformation($"Caught new update: Id: {update.Id, -15}, Author: {update.Author.Name, -15}, Created at: {update.CreatedAt}");
            foreach (long chatId in _config.TelegramBotConfig.UpdateChatsIds)
            {
                await SendMessage(update, chatId);
            }
        }

        private async Task SendMessage(Update update, long chatId)
        {
            try
            {
                if (_validator.WasUpdateSent(update.Id, chatId))
                {
                    _logger.LogInformation($"Update #{update.Id} was already sent to chat #{chatId}");
                    return;
                }

                await _sender.SendAsync(update, chatId);
                
                _validator.UpdateSent(update.Id, chatId);
                
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