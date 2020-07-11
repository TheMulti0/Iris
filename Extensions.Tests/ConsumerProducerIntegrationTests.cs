using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Extensions.Tests
{
    /// <summary>
    ///     Requires a Kafka broker running
    /// </summary>
    [TestClass]
    public class ConsumerProducerIntegrationTests
    {
        private const string Topic = "tests";
        private const string UpdateContent = "This is a test!";

        private static BaseKafkaConfig _config;
        private static ILoggerFactory _loggerFactory;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _config = JsonSerializer.Deserialize<BaseKafkaConfig>(
                File.ReadAllText("../../../appsettings.json"),
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                });
            
            _loggerFactory = LoggerFactory.Create(
                builder => builder.AddProvider(new TestsLoggerProvider(context)));
        }

        [TestMethod]
        public async Task TestProduceConsume()
        {
            ProduceOneMessage(UpdateContent);
            Result<Message<Unit, Update>> firstMessage = await GetConsumedMessages().FirstOrDefaultAsync();

            Assert.AreEqual(UpdateContent, firstMessage.Value.Value.Value.Content);
        }

        private static IObservable<Result<Message<Unit, Update>>> GetConsumedMessages()
        {
            var config = new ConsumerConfig
            {
                Topics = new[]
                {
                    Topic
                },
                BrokersServers = _config.BrokersServers,
                GroupId = "tests-consumers-group",
                KeySerializationType = _config.KeySerializationType,
                ValueSerializationType = _config.ValueSerializationType
            };

            var consumer = new Consumer<Unit, Update>(
                config,
                _loggerFactory);

            return consumer.Messages;
        }

        private static void ProduceOneMessage(string updateContent)
        {
            var producer = new Producer<Unit, Update>(
                _config,
                _loggerFactory);
            
            producer.Produce(
                Topic,
                new Update
                {
                    Content = updateContent
                });
        }

        private class Update
        {
            public string Content { get; set; }
        }
    }
}