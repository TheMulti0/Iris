using System;
using Kafka.Public;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddProducer<TKey, TValue>(
            this IServiceCollection services,
            BaseKafkaConfig config) 
            where TKey : class 
            where TValue : class
        {
            IKafkaProducer<TKey, TValue> CreateKafkaProducer(IServiceProvider provider)
            {
                return KafkaProducerFactory.Create<TKey, TValue>(config, provider.GetService<ILoggerFactory>());
            }

            return services.AddSingleton(CreateKafkaProducer);
        }
        
        public static IServiceCollection AddProducer<TKey, TValue>(
            this IServiceCollection services) 
            where TKey : class 
            where TValue : class
        {
            IKafkaProducer<TKey, TValue> CreateKafkaProducer(IServiceProvider provider)
            {
                return KafkaProducerFactory.Create<TKey, TValue>(
                    provider.GetService<BaseKafkaConfig>(),
                    provider.GetService<ILoggerFactory>());
            }

            return services.AddSingleton(CreateKafkaProducer);
        }
        
        public static IServiceCollection AddConsumer<TKey, TValue>(
            this IServiceCollection services,
            ConsumerConfig config) 
            where TKey : class 
            where TValue : class
        {
            IKafkaConsumer<TKey, TValue> CreateKafkaConsumer(IServiceProvider provider)
            {
                return KafkaConsumerFactory.Create<TKey, TValue>(
                    config,
                    provider.GetService<ILoggerFactory>());
            }

            return services.AddSingleton(
                CreateKafkaConsumer);
        }
        
        public static IServiceCollection AddConsumer<TKey, TValue>(
            this IServiceCollection services) 
            where TKey : class 
            where TValue : class
        {
            static IKafkaConsumer<TKey, TValue> CreateKafkaConsumer(IServiceProvider provider)
            {
                return KafkaConsumerFactory.Create<TKey, TValue>(
                    provider.GetService<ConsumerConfig>(),
                    provider.GetService<ILoggerFactory>());
            }

            return services.AddSingleton(
                CreateKafkaConsumer);
        }
    }
}