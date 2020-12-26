﻿using System.Text.Json;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace MessagesManager
{
    public class MessagesProducer : IProducer<Message>
    {
        private readonly RabbitMqConfig _config;
        private readonly RabbitMqPublisher _publisher;
        private readonly ILogger<MessagesProducer> _logger;

        public MessagesProducer(
            RabbitMqConfig config,
            ILogger<MessagesProducer> logger)
        {
            _config = config;
            _publisher = new RabbitMqPublisher(config);
            _logger = logger;
        }
        
        public void Send(Message message)
        {
            _logger.LogInformation("Sending message {}", message);
            
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(message);
            
            _publisher.Publish(_config.Destination, bytes);
        }
    }
}