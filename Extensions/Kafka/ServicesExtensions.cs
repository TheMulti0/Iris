using System;
using System.Text.Json;
using Kafka.Public;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddProducer<TKey, TValue>(
            this IServiceCollection services,
            JsonSerializerOptions options = null) 
            where TKey : class 
            where TValue : class
        {
            IKafkaProducer<TKey, TValue> CreateKafkaProducer(IServiceProvider provider)
            {
                return KafkaProducerFactory.Create<TKey, TValue>(
                    provider.GetService<BaseKafkaConfig>(),
                    provider.GetService<ILoggerFactory>(),
                    options);
            }

            return services.AddSingleton(CreateKafkaProducer);
        }
        
        public static IServiceCollection AddConsumer<TKey, TValue>(
            this IServiceCollection services,
            ConsumerConfig config,
            JsonSerializerOptions options = null) 
            where TKey : class 
            where TValue : class
        {
            IKafkaConsumer<TKey, TValue> CreateKafkaConsumer(IServiceProvider provider)
            {
                return KafkaConsumerFactory.Create<TKey, TValue>(
                    config,
                    provider.GetService<ILoggerFactory>(),
                    options);
            }

            return services.AddSingleton(CreateKafkaConsumer);
        }
    }
}