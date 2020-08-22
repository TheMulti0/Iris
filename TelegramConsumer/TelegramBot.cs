using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace TelegramConsumer
{
    public class TelegramBot
    {
        private readonly ITelegramBotClientProvider _clientProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<TelegramBot> _logger;
        private readonly ConcurrentDictionary<long, ActionBlock<MessageInfo>> _chatSenders;

        private ITelegramBotClient _client;
        private MessageSender _sender;
        private TelegramConfig _config;

        private CancellationTokenSource _sendCancellation;

        public TelegramBot(
            IConfigProvider configProvider,
            ITelegramBotClientProvider clientProvider,
            ILoggerFactory loggerFactory)
        {
            _clientProvider = clientProvider;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<TelegramBot>();
            _chatSenders = new ConcurrentDictionary<long, ActionBlock<MessageInfo>>();

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
                Cancel();

                _client = client.Value;
                _sender = new MessageSender(_client, _loggerFactory);
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

        private void ClearChatSenders()
        {
            _chatSenders.Clear();
            _logger.LogInformation("Cleared all chat senders");
        }

        private void CompleteChatSenders()
        {
            foreach ((long chatId, ActionBlock<MessageInfo> chatSender) in _chatSenders)
            {
                _logger.LogInformation("Disposing chat sender for chat id: {}", chatId);
                chatSender.Complete();
            }
        }

        public async Task SendAsync(Update update)
        {
            if (_config == null)
            {
                _logger.LogError("Update request sent, but no config present. Leaving.");
                return;
            }
            if (!TryGetUser(update.AuthorId, out User user))
            {
                _logger.LogError("User {} is not in config. Leaving.", update.AuthorId);
                return;
            }

            _logger.LogInformation("Sending update {}", update);
            
            string updateMessage = MessageBuilder.Build(update, user);

            foreach (long chatId in user.ChatIds)
            {
                ActionBlock<MessageInfo> chatSender = _chatSenders
                    .GetOrAdd(chatId, id => new ActionBlock<MessageInfo>(_sender.SendAsync));
                
                var messageInfo = new MessageInfo(
                    updateMessage,
                    update.Media,
                    chatId,
                    _sendCancellation.Token);
                
                await chatSender.SendAsync(messageInfo);
            }
        }

        private bool TryGetUser(string authorId, out User user)
        {
            user = _config.Users.FirstOrDefault(u => u.UserName == authorId);
            return user != null;
        }

        public async ValueTask WaitForCompleteAsync()
        {
            Task[] completions = _chatSenders.Values
                .Select(sender => sender.Completion)
                .ToArray();

            await Task.WhenAll(completions);
            
            ClearChatSenders();
        }

        public void Cancel()
        {
            CancelSendOperations();
            CompleteChatSenders();
            
            ClearChatSenders();
        }
    }
}