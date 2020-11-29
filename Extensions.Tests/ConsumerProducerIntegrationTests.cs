using System;
using System.IO;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kafka.Public;
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
                builder => builder.AddTestsLogging(context));
        }

        [TestMethod]
        public async Task TestProduceConsume()
        {
            ProduceOneMessage(UpdateContent);
            KafkaRecord<Nothing, Update> firstMessage = await GetConsumedMessages().FirstOrDefaultAsync();

            Assert.AreEqual(UpdateContent, firstMessage.Value.Content);
        }

        private static IObservable<KafkaRecord<Nothing, Update>> GetConsumedMessages()
        {
            var config = new ConsumerConfig
            {
                SubscriptionTopics = new[]
                {
                    _config.DefaultTopic
                },
                DefaultTopic = _config.DefaultTopic,
                BrokersServers = _config.BrokersServers,
                GroupId = "tests-consumers-group",
                KeySerializationType = _config.KeySerializationType,
                ValueSerializationType = _config.ValueSerializationType
            };

            var consumer = KafkaConsumerFactory.Create<Nothing, Update>(
                config,
                _loggerFactory,
                new JsonSerializerOptions());

            return consumer.Messages;
        }

        private static void ProduceOneMessage(string updateContent)
        {
            var producer = KafkaProducerFactory.Create<Nothing, Update>(
                _config,
                _loggerFactory,
                new JsonSerializerOptions());
            
            producer.Produce(
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