using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConcurrentDictionary<long, ActionBlock<Task>> _chatSenders;
        
        private readonly object _configLock = new object();
        private MessageSender _sender;
        private TelegramConfig _config;

        public TelegramBot(
            IConfigProvider configProvider,
            ITelegramBotClientProvider clientProvider,
            ILoggerFactory loggerFactory)
        {
            _clientProvider = clientProvider;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<TelegramBot>();
            _chatSenders = new ConcurrentDictionary<long, ActionBlock<Task>>();
            
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
                _sender = new MessageSender(client.Value, _loggerFactory);
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

        public async Task SendAsync(Update update)
        {
            if (!TryGetUser(update.AuthorId, out User user))
            {
                _logger.LogError("User {} is not in config. Leaving.", update.AuthorId);
                return;
            }

            _logger.LogInformation("Sending update {}", update);
            
            string updateMessage = MessageBuilder.Build(update, user);

            foreach (long chatId in user.ChatIds)
            {
                await SendChatUpdate(updateMessage, update.Media, chatId);
            }
        }

        private async Task SendChatUpdate(
            string message,
            Media[] media,
            long chatId)
        {
            ActionBlock<Task> chatSender = _chatSenders
                .GetOrAdd(chatId, id => new ActionBlock<Task>(task => task));

            var messageInfo = new MessageInfo(
                message,
                media,
                chatId);

            await chatSender.SendAsync(
                _sender.SendAsync(messageInfo));
        }

        private bool TryGetUser(string authorId, out User user)
        {
            lock (_configLock)
            {
                if (_config == null)
                {
                    _logger.LogError("Update request received, but no config present");
                }
                
                user = _config?.Users.FirstOrDefault(u => u.UserName == authorId);
                return user != null;
            }
        }

        public async Task FlushAsync()
        {
            Task[] completions = _chatSenders
                .Select(CompleteAsync)
                .ToArray();

            await Task.WhenAll(completions);
        }

        private Task CompleteAsync(KeyValuePair<long, ActionBlock<Task>> pair)
        {
            (long chatId, ActionBlock<Task> chatSender) = pair;

            _logger.LogInformation("Completing chat sender for chat id: {}", chatId);

            return chatSender.CompleteAsync();
        }
    }
}