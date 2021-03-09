using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddRabbitMqConnection(
            this IServiceCollection services,
            RabbitMqConnectionConfig config)
        {
            IModel CreateConnection(IServiceProvider _)
            {
                var factory = new ConnectionFactory
                {
                    Uri = config.ConnectionString,
                    DispatchConsumersAsync = true,
                    ConsumerDispatchConcurrency = config.ConcurrencyLevel
                };
                
                return factory
                    .CreateConnection()
                    .CreateModel();
            }

            return services.AddSingleton(CreateConnection);
        }
        
        public static IServiceCollection AddProducer<T>(
            this IServiceCollection services,
            RabbitMqProducerConfig config)
        {
            Producer<T> CreateProducer(IServiceProvider provider)
            {
                return new(
                    config,
                    provider.GetService<IModel>(),
                    provider.GetService<ILogger<Producer<T>>>());
            }

            return services.AddSingleton<IProducer<T>, Producer<T>>(CreateProducer);
        }
        
        public static IServiceCollection AddConsumerService<T>(
            this IServiceCollection services,
            RabbitMqConsumerConfig config)
        {
            ConsumerService<T> CreateService(IServiceProvider provider)
            {
                return new(
                    config,
                    provider.GetService<IModel>(),
                    provider.GetService<IConsumer<T>>(),
                    provider.GetService<ILoggerFactory>());
            }

            return services.AddHostedService(CreateService);
        }
    }
}