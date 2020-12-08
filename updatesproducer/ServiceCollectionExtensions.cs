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
                configuration.GetSection<KafkaConfig>("Kafka"),
                configuration.GetSection<PollerConfig>("Poller"),
                configuration.GetSection<VideoExtractorConfig>("VideoExtractor"));
        }

        public static IServiceCollection AddUpdatesProducer<TProvider>(
            this IServiceCollection services,
            MongoDbConfig mongoDbConfig,
            KafkaConfig kafkaConfig,
            PollerConfig pollerConfig,
            VideoExtractorConfig videoExtractorConfig) where TProvider : class, IUpdatesProvider
        {
            services = mongoDbConfig != null 
                ? services.AddUpdatesProducerMongoRepositories(mongoDbConfig) 
                : services.AddUpdatesProducerMockRepositories();

            var baseKafkaConfig = new BaseKafkaConfig
            {
                BrokersServers = kafkaConfig.BrokersServers,
                Topic = kafkaConfig.Updates.Topic,
                KeySerializationType = SerializationType.String,
                ValueSerializationType = SerializationType.Json
            };
            
            return services
                .AddProducer<string, Update>(baseKafkaConfig)
                .AddSingleton<IUpdatesProducer, KafkaUpdatesProducer>()
                .AddVideoExtractor(videoExtractorConfig)
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

        public static IServiceCollection AddVideoExtractor(
            this IServiceCollection services,
            VideoExtractorConfig config)
        {
            return services
                .AddSingleton(config)
                .AddSingleton<VideoExtractor>();
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