using Microsoft.Extensions.DependencyInjection;

namespace Consumer
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddConsumer<TKey, TValue>(
            this IServiceCollection services,
            ConsumerConfig config)
        {
            return services.AddSingleton(
                s => new Consumer<TKey, TValue>(config));
        }
    }
}