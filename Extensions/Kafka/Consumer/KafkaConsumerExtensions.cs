using System.Threading.Tasks;
using Kafka.Public;

namespace Extensions
{
    public static class KafkaConsumerExtensions
    {
        public static Task CommitAsync<TKey, TValue>(
            this IKafkaConsumer<TKey, TValue> consumer,
            KafkaRecord<TKey, TValue> record) 
            where TKey : class where TValue : class
        {
            return consumer.CommitAsync(record.Topic, record.Partition, record.Offset);
        }
    }
}