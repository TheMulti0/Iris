using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbGenericRepository;
using UpdatesProducer.Mock;

namespace UpdatesProducer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUpdatesProducer<TProvider>(
            this IServiceCollection services,
            IConfiguration configuration) where TProvider : class, IUpdatesProvider
        {
            return services.AddUpdatesProducer<TProvider>(
                configuration.GetSection<MongoDbConfig>("MongoDb"),
                configuration.GetSection<RabbitMqConfig>("UpdatesPublisher"),
                configuration.GetSection<UpdatesProviderBaseConfig>("UpdatesProvider"),
                configuration.GetSection<PollerConfig>("Poller"),
                configuration.GetSection<VideoExtractorConfig>("VideoExtractor"));
        }

        public static IServiceCollection AddUpdatesProducer<TProvider>(
            this IServiceCollection services,
            MongoDbConfig mongoDbConfig,
            RabbitMqConfig rabbitMqConfig,
            UpdatesProviderBaseConfig updatesProviderBaseConfig,
            PollerConfig pollerConfig,
            VideoExtractorConfig videoExtractorConfig) where TProvider : class, IUpdatesProvider
        {
            services = mongoDbConfig != null 
                ? services.AddUpdatesProducerMongoRepositories(mongoDbConfig) 
                : services.AddUpdatesProducerMockRepositories();
            
            return services
                .AddRabbitMqUpdatesPublisher(rabbitMqConfig)
                .AddVideoExtractor(videoExtractorConfig)
                .AddUpdatesProvider<TProvider>(updatesProviderBaseConfig)
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
            var mongoDbContext = new MongoDbContext(config.ConnectionString, config.DatabaseName);
            
            return services
                .AddSingleton<IMongoDbContext>(mongoDbContext)
                .AddSingleton(config);
        }

        public static IServiceCollection AddRabbitMqUpdatesPublisher(
            this IServiceCollection services,
            RabbitMqConfig config)
        {
            return services
                .AddSingleton(config)
                .AddSingleton<RabbitMqPublisher>()
                .AddSingleton<IUpdatesPublisher, RabbitMqUpdatesPublisher>();
        }
        

        public static IServiceCollection AddVideoExtractor(
            this IServiceCollection services,
            VideoExtractorConfig config)
        {
            return services
                .AddSingleton(config)
                .AddSingleton<VideoExtractor>();
        }

        public static IServiceCollection AddUpdatesProvider<TProvider>(
            this IServiceCollection services,
            UpdatesProviderBaseConfig config) where TProvider : class, IUpdatesProvider
        {
            return services
                .AddSingleton(config)
                .AddSingleton<IUpdatesProvider, TProvider>();
        }

        public static IServiceCollection AddUpdatesPollerService(
            this IServiceCollection services,
            PollerConfig config)
        {
            return services
                .AddSingleton(config)
                .AddHostedService<UpdatesPollerService>();
        }
    }
}