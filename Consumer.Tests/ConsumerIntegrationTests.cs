using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Consumer.Tests
{
    /// <summary>
    /// Requires a Kafka broker running 
    /// </summary>
    [TestClass]
    public class ConsumerIntegrationTests
    {
        private class Config
        {
            public string BrokersServers { get; set; }
        }

        private class UpdateSerializer : ISerializer
        {
            public int Serialize(object input, MemoryStream toStream)
            {
                byte[] data = JsonSerializer.SerializeToUtf8Bytes(input);
                toStream.Write(data);
                return data.Length;
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

            Assert.AreEqual(UpdateContent, messages?.Value?.Value.Content);
        }

        private static IObservable<Result<Message<Unit, Update>>> GetConsumedMessages()
        {
            var config = new ConsumerConfig
            {
                Topics = new [] { Topic },
                PollIntervalSeconds = IntervalSeconds,
                BrokersServers = _config.BrokersServers,
                GroupId = "tests-consumers-group"
            };

            var consumer = new Consumer<Unit, Update>(
                config);
            
            return consumer.Messages;
        }

        private static void ProduceOneMessage(string updateContent)
        {
            var serializationConfig = new SerializationConfig();
            serializationConfig.SetDefaultSerializers(
                new UpdateSerializer(), 
                new UpdateSerializer());
            var config = new Configuration
            {
                Seeds = _config.BrokersServers,
                SerializationConfig = serializationConfig
            };

            var cluster = new ClusterClient(config, new ConsoleLogger());
            cluster.Produce(
                Topic,
                new Update
                {
                    Content = updateContent
                });
        }
    }
}