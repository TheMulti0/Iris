using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Message = Telegram.Bot.Types.Message;
using User = Common.User;

namespace TelegramReceiver
{
    public class MessageReceiverService : BackgroundService
    {
        private readonly ITelegramBotClient _client;
        private readonly IChatPollRequestsProducer _producer;
        private readonly ILogger<MessageReceiverService> _logger;

        public MessageReceiverService(
            TelegramConfig config,
            IChatPollRequestsProducer producer,
            ILogger<MessageReceiverService> logger)
        {
            _client = new TelegramBotClient(config.AccessToken);
            _producer = producer;
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
            
            _client.OnUpdate += ClientOnOnUpdate;
        }

        private void ClientOnOnUpdate(object sender, UpdateEventArgs e)
        {
            Message updateMessage = e.Update.Message;

            if (updateMessage == null)
            {
                return;
            }
            
            _producer.SendRequest(
                new ChatPollRequest(
                    Request.StartPoll,
                    new UserPollRule(
                        new User(updateMessage.Text, updateMessage.Text, "Facebook"),
                        TimeSpan.FromSeconds(10)),
                    (ChatId) updateMessage.Chat.Id));
        }

        public override void Dispose()
        {
            _client.StopReceiving();
        }
    }
}