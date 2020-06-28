using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Producer
{
    class Update 
    {
        public string Content { get; set; }
    }

    class UpdateSerializer : ISerializer<Update>
    {
        public byte[] Serialize(Update data, SerializationContext context)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data);
        }
    }

    class Program
    {
        private static async Task Main(string[] args)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            // Create a producer that can be used to send messages to kafka that have no key and a value of type string 
            using var p = new ProducerBuilder<Null, Update>(config).SetValueSerializer(new UpdateSerializer()).Build();

            var i = 0;
            while (true)
            {
                // Construct the message to send (generic type must match what was used above when creating the producer)
                var message = new Message<Null, Update>
                {
                    Value = new Update 
                    {
                        Content = $"Message #{++i}"
                    }
                };

                // Send the message to our test topic in Kafka                
                var dr = await p.ProduceAsync("test-topic", message);
                Console.WriteLine($"Produced message '{dr.Value}' to topic {dr.Topic}, partition {dr.Partition}, offset {dr.Offset}");

                await Task.Delay(TimeSpan.FromMilliseconds(5000));
            }
        }
    }
}
