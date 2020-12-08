using Common;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDbGenericRepository;
using UpdatesProducer.Mock;

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
            services = mongoDbConfig != null 
                ? services.AddUpdatesProducerMongoRepositories(mongoDbConfig) 
                : services.AddUpdatesProducerMockRepositories();

            return services
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
                .AddSingleton<MongoApplicationDbContext>()
                .AddSingleton<IUserLatestUpdateTimesRepository, MongoUserLatestUpdateTimesRepository>()
                .AddSingleton<ISentUpdatesRepository, MongoSentUpdatesRepository>();
        }

        public static IServiceCollection AddUpdatesProducerMockRepositories(
            this IServiceCollection services)
        {
            return services
                .AddSingleton<IUserLatestUpdateTimesRepository, MockUserLatestUpdateTimesRepository>()
                .AddSingleton<ISentUpdatesRepository, MockSentUpdatesRepository>();
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