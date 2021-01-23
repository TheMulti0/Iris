using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Extensions.Tests
{
    [TestClass]
    public class RabbitMqIntegrationTests
    {
        private const string UpdateContent = "This is a test!";

        private static RabbitMqProducerConfig _producerConfig;
        private static RabbitMqConsumerConfig _consumerConfig;
        private static ILoggerFactory _loggerFactory;
        private static IModel _channel;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _producerConfig = new RabbitMqProducerConfig
            {
                Exchange = "updates"
            };
            
            _consumerConfig = new RabbitMqConsumerConfig
            {
                Queue = "updates"
            };

            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672//")
            };
            
            _channel = connectionFactory
                .CreateConnection()
                .CreateModel();
            
            _loggerFactory = LoggerFactory.Create(
                builder => builder.AddTestsLogging(context));
        }

        [TestMethod]
        public async Task TestPublishConsume()
        {
            var update = await ConsumeProducedUpdate();
            
            Assert.AreEqual(UpdateContent, update.Content);
        }

        private static async Task<Update> ConsumeProducedUpdate()
        {
            var cs = new TaskCompletionSource<BasicDeliverEventArgs>();

            Task OnMessage(BasicDeliverEventArgs args)
            {
                cs.TrySetResult(args);

                return Task.CompletedTask;
            }
            
            PublishOneMessage(UpdateContent);
            RabbitMqConsumer consumer = Consume(OnMessage);
            BasicDeliverEventArgs message = await cs.Task;
            
            var update = JsonSerializer.Deserialize<Update>(message.Body.Span.ToArray());
            
            consumer.Dispose(); // Stop receiving messages in this scope

            return update;
        }
        
        private static RabbitMqConsumer Consume(Func<BasicDeliverEventArgs, Task> onMessage)
        {
            return new RabbitMqConsumer(
                _consumerConfig,
                _channel,
                onMessage,
                _loggerFactory.CreateLogger<RabbitMqConsumer>());
        }

        private static void PublishOneMessage(string updateContent)
        {
            var producer = new Producer<Update>(
                _producerConfig,
                _channel,
                _loggerFactory.CreateLogger<Producer<Update>>());

            var update = new Update
            {
                Content = updateContent
            };
            
            producer.Send(update);
        }

        private class Update
        {
            public string Content { get; set; }
        }
    }
}