using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddProducer<T>(
            this IServiceCollection services,
            RabbitMqConfig config)
        {
            return services.AddSingleton<IProducer<T>, Producer<T>>(
                    provider => new Producer<T>(
                        config,
                        provider.GetService<ILogger<Producer<T>>>()));
        }
        
        public static IServiceCollection AddConsumerService<T>(
            this IServiceCollection services,
            RabbitMqConfig config)
        {
            ConsumerService<T> CreateService(IServiceProvider provider)
            {
                return new(
                    config,
                    provider.GetService<IConsumer<T>>(),
                    provider.GetService<ILogger<ConsumerService<T>>>());
            }

            return services.AddHostedService(CreateService);
        }
    }
}