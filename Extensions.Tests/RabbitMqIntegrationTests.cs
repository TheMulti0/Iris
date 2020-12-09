using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client.Events;

namespace Extensions.Tests
{
    [TestClass]
    public class RabbitMqIntegrationTests
    {
        private const string UpdateContent = "This is a test!";

        private static RabbitMqConfig _publisherConfig;
        private static RabbitMqConfig _consumerConfig;
        private static ILoggerFactory _loggerFactory;

        private const string Exchange = "amq.topic";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _publisherConfig = new RabbitMqConfig
            {
                ConnectionString = new Uri("amqp://guest:guest@localhost:5672//"),
                Destination = "amq.topic"
            };
            
            _consumerConfig = new RabbitMqConfig
            {
                ConnectionString = new Uri("amqp://guest:guest@localhost:5672//"),
                Destination = "updates"
            };
            
            _loggerFactory = LoggerFactory.Create(
                builder => builder.AddTestsLogging(context));
        }

        [TestMethod]
        public async Task TestPublishConsume()
        {
            PublishOneMessage(UpdateContent);
            
            BasicDeliverEventArgs firstMessage = null;
            IObservable<BasicDeliverEventArgs> consumedMessages = GetConsumedMessages();
            consumedMessages.Subscribe(
                args =>
                {
                    firstMessage = args;
                });
            await Task.Delay(1000);            
            var update = JsonSerializer.Deserialize<Update>(firstMessage.Body.Span);

            Assert.AreEqual(UpdateContent, update.Content);
        }

        private static IObservable<BasicDeliverEventArgs> GetConsumedMessages()
        {
            var consumer = new RabbitMqConsumer(_consumerConfig);

            return consumer.Messages;
        }

        private static void PublishOneMessage(string updateContent)
        {
            var producer = new RabbitMqPublisher(_publisherConfig);

            var update = new Update
            {
                Content = updateContent
            };
            
            producer.Publish(
                "update",
                JsonSerializer.SerializeToUtf8Bytes(update));
        }

        private class Update
        {
            public string Content { get; set; }
        }
    }
}