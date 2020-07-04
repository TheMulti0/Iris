using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddConsumer<TKey, TValue>(
            this IServiceCollection services,
            ConsumerConfig config)
        {
            return services.AddSingleton(
                s => new Consumer<TKey, TValue>(config, s.GetService<ILoggerFactory>()));
        }
    }
}