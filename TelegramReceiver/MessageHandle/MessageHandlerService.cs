using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal static class Routes
    {
        public const string UserRoute = "user";
    }
    
    public class MessageHandlerService : BackgroundService
    {
        private readonly ITelegramBotClient _client;
        private readonly IEnumerable<ICommand> _commands;
        private readonly IChatPollRequestsProducer _producer;
        private readonly ILogger<MessageHandlerService> _logger;

        public MessageHandlerService(
            TelegramConfig config,
            IEnumerable<ICommand> commands,
            //IChatPollRequestsProducer producer,
            ILogger<MessageHandlerService> logger)
        {
            _client = new TelegramBotClient(config.AccessToken);
            _commands = commands;
            //_producer = producer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var identity = await _client.GetMeAsync(stoppingToken);

            _logger.LogInformation(
                "Logged into Telegram as {} {} (Username = {}, Id = {})",
                identity.FirstName,
                identity.LastName,
                identity.Username,
                identity.Id);
            
            _client.StartReceiving(cancellationToken: stoppingToken);
            
            _client.OnUpdate += OnUpdate;
        }

        private async void OnUpdate(object _, UpdateEventArgs args)
        {
            Update update = args.Update;

            foreach (ICommand command in _commands)
            {
                bool shouldTrigger = command.Triggers
                    .Any(trigger => trigger.ShouldTrigger(update));
                
                if (shouldTrigger)
                {
                    await command.OperateAsync(_client, update);
                }
            }
        }

        public override void Dispose()
        {
            _client.StopReceiving();
        }
    }
}