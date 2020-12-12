using Microsoft.Extensions.DependencyInjection;

namespace Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddPublisher(
            this IServiceCollection services,
            RabbitMqConfig config)
        {
            return services.AddSingleton(config)
                .AddSingleton<RabbitMqPublisher>();
        }
        
        public static IServiceCollection AddConsumer(
            this IServiceCollection services,
            RabbitMqConfig config)
        {
            return services.AddSingleton(config)
                .AddSingleton<RabbitMqConsumer>();
        }
    }
}