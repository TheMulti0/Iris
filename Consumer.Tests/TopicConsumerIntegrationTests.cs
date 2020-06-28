using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Consumer.Tests
{
    /// <summary>
    /// TopicConsumerIntegrationTests require Kafka broker running 
    /// </summary>
    [TestClass]
    public class TopicConsumerIntegrationTests
    {
        class Config
        {
            public string BootstrapServers { get; set; }
        }
        
        class UpdateSerializer : ISerializer<Update>
        {
            public byte[] Serialize(Update data, SerializationContext context)
            {
                return JsonSerializer.SerializeToUtf8Bytes(data);
            }
        }

        private static Config _config;
        private const string Topic = "tests";
        private const double IntervalSeconds = 0.5;
        private const string UpdateContent = "This is a test!";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _config = JsonSerializer.Deserialize<Config>(
                File.ReadAllText("../../../appsettings.json"));
        }
        
        [TestMethod]
        public async Task TestConsumerResponse()
        {
            ProduceOneMessage(UpdateContent);
            var messages = await GetConsumedMessages().FirstOrDefaultAsync();

            Assert.AreEqual(UpdateContent, messages?.Message?.Value?.Content);
        }

        private static IObservable<ConsumeResult<Ignore, Update>> GetConsumedMessages()
        {
            var config = new TopicConsumerConfig
            {
                Topic = Topic,
                PollIntervalSeconds = IntervalSeconds,
                BootstrapServers = _config.BootstrapServers,
                GroupId = "tests-consumers-group"
            };

            var consumer = new TopicConsumer<Ignore, Update>(config);
            return consumer.Messages;
        }

        private static void ProduceOneMessage(string updateContent)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _config.BootstrapServers
            };

            using IProducer<Null, Update> producer = new ProducerBuilder<Null, Update>(config)
                .SetValueSerializer(new UpdateSerializer())
                .Build();

            var message = new Message<Null, Update>
            {
                Value = new Update
                {
                    Content = updateContent
                }
            };
            producer.Produce(Topic, message);
        }
    }
}