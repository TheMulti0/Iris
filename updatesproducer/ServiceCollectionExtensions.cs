using Common;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDbGenericRepository;

namespace UpdatesProducer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUpdatesProducer<TProvider>(
            this IServiceCollection services,
            MongoDbConfig mongoDbConfig,
            BaseKafkaConfig kafkaConfig,
            PollerConfig pollerConfig) where TProvider : class, IUpdatesProvider
        {
            return services
                .AddUpdatesProducerMongoRepositories(mongoDbConfig)
                .AddProducer<string, Update>(kafkaConfig)
                .AddSingleton<IUpdatesProducer, KafkaUpdatesProducer>()
                .AddSingleton<IUpdatesProvider, TProvider>()
                .AddUpdatesPollerService(pollerConfig);
        }

        public static IServiceCollection AddUpdatesProducerMongoRepositories(
            this IServiceCollection services,
            MongoDbConfig config)
        {
            return services
                .AddMongoDb(config)
                .AddSingleton<ApplicationDbContext>()
                .AddSingleton<IUserLatestUpdateTimesRepository, UserLatestUpdateTimesRepository>()
                .AddSingleton<ISentUpdatesRepository, SentUpdatesRepository>();
        }

        public static IServiceCollection AddMongoDb(
            this IServiceCollection services,
            MongoDbConfig config)
        {
            return services.AddSingleton<IMongoDbContext>(
                new MongoDbContext(config.ConnectionString, config.DatabaseName));
        }

        public static IServiceCollection AddUpdatesPollerService(
            this IServiceCollection services,
            PollerConfig pollerConfig)
        {
            return services
                .AddSingleton(pollerConfig)
                .AddHostedService<UpdatesPollerService>();
        }
    }
}