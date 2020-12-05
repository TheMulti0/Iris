using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDbGenericRepository;

namespace UpdatesProducer
{
    public static class MongoDbBuilderExtensions
    {
        public static IServiceCollection AddMongoDb(
            this IServiceCollection services,
            MongoDbSettings settings)
        {
            services.TryAddSingleton<IMongoDbContext>(
                new MongoDbContext(settings.ConnectionString, settings.DatabaseName));

            return services;
        }
    }
}