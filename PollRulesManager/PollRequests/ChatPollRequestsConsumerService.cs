using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using UserDataLayer;

namespace PollRulesManager
{
    public class ChatPollRequestsConsumerService : BackgroundService
    {
        private readonly RabbitMqConfig _config;
        private readonly IChatPollRequestsConsumer _consumer;
        private readonly ISavedUsersRepository _repository;
        private readonly ILogger<ChatPollRequestsConsumerService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ChatPollRequestsConsumerService(
            RabbitMqConfig config, 
            IChatPollRequestsConsumer consumer,
            ISavedUsersRepository repository,
            ILogger<ChatPollRequestsConsumerService> logger)
        {
            _config = config;
            _consumer = consumer;
            _repository = repository;
            _logger = logger;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new TimeSpanConverter(),
                    new MediaJsonConverter()
                }
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new RabbitMqConsumer(_config, OnMessage(stoppingToken));

            // Dispose the consumer when service is stopped
            stoppingToken.Register(() => consumer.Dispose());

            await _repository.Get()
                .AsEnumerable()
                .SelectMany(ToChatPollRequests)
                .ToAsyncEnumerable()
                .ForEachAwaitWithCancellationAsync(_consumer.OnRequestAsync, stoppingToken);
        }

        private static IEnumerable<ChatPollRequest> ToChatPollRequests(SavedUser user)
        {
            ChatPollRequest ToChatPollRequest(ChatInfo info)
            {
                var userPollRule = new UserPollRule(user.User, info.Interval);
                
                return new ChatPollRequest(
                    Request.StartPoll,
                    userPollRule,
                    info.Chat);
            }

            return user.Chats.Select(ToChatPollRequest);
        }

        private Func<BasicDeliverEventArgs, Task> OnMessage(CancellationToken token)
        {
            return async message =>
            {
                try
                {
                    string json = Encoding.UTF8.GetString(message.Body.Span.ToArray());
                    
                    var request = JsonSerializer.Deserialize<ChatPollRequest>(json, _jsonSerializerOptions)
                                 ?? throw new NullReferenceException($"Failed to deserialize {json}");
                    
                    await SaveRequestAsync(request);

                    await _consumer.OnRequestAsync(request, token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "");
                }
            };
        }

        private async Task SaveRequestAsync(ChatPollRequest request)
        {
            User pollRuleUser = request.PollRule.User;
            var chatInfo = new ChatInfo
            {
                Interval = request.PollRule.Interval ?? throw new NullReferenceException(),
                Chat = request.ChatId
            };

            if (request.Request == Request.StartPoll)
            {
                await _repository.AddOrUpdateAsync(
                    pollRuleUser,
                    chatInfo);
            }
            else
            {
                await _repository.RemoveAsync(
                    pollRuleUser,
                    chatInfo);
            }
        }
    }
}