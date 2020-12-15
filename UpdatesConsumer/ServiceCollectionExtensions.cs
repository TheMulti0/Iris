using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UpdatesConsumer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUpdatesConsumer<TConsumer>(
            this IServiceCollection services,
            IConfiguration configuration) where TConsumer : class, IUpdateConsumer
        {
            return services.AddUpdatesConsumer<TConsumer>(
                configuration.GetSection<RabbitMqConfig>("UpdatesConsumer"));
        }

        public static IServiceCollection AddUpdatesConsumer<TConsumer>(
            this IServiceCollection services,
            RabbitMqConfig rabbitMqConfig) where TConsumer : class, IUpdateConsumer
        {
            return services
                .AddRabbitMqConsumer(rabbitMqConfig)
                .AddSingleton<IUpdateConsumer, TConsumer>()
                .AddHostedService<UpdatesConsumerService>();
        }

        public static IServiceCollection AddRabbitMqConsumer(
            this IServiceCollection services,
            RabbitMqConfig config)
        {
            return services
                .AddSingleton(config)
                .AddSingleton<RabbitMqConsumer>();
        }
    }
}