using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDbGenericRepository;
using UpdatesScraper.Mock;

namespace UpdatesScraper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUpdatesScraper<TProvider>(
            this IServiceCollection services,
            IConfiguration configuration) where TProvider : class, IUpdatesProvider
        {
            return services.AddUpdatesScraper<TProvider>(
                configuration.GetSection<MongoDbConfig>("MongoDb"),
                configuration.GetSection<RabbitMqConfig>("UpdatesProducer"),
                configuration.GetSection<UpdatesProviderBaseConfig>("UpdatesProvider"),
                configuration.GetSection<VideoExtractorConfig>("VideoExtractor"),
                configuration.GetSection<ScraperConfig>("Scraper"),
                configuration.GetSection<RabbitMqConfig>("JobsConsumer"));
        }

        public static IServiceCollection AddUpdatesScraper<TProvider>(
            this IServiceCollection services,
            MongoDbConfig mongoDbConfig,
            RabbitMqConfig producerConfig,
            UpdatesProviderBaseConfig updatesProviderBaseConfig,
            VideoExtractorConfig videoExtractorConfig,
            ScraperConfig scraperConfig,
            RabbitMqConfig consumerConfig) where TProvider : class, IUpdatesProvider
        {
            services = mongoDbConfig != null 
                ? services.AddUpdatesScraperMongoRepositories(mongoDbConfig) 
                : services.AddUpdatesScraperMockRepositories();

            return services
                .AddUpdatesProducer(producerConfig)
                .AddUpdatesProvider<TProvider>(updatesProviderBaseConfig)
                .AddVideoExtractor(videoExtractorConfig)
                .AddUpdatesScraper(scraperConfig)
                .AddJobsConsumer(consumerConfig);
        }

        public static IServiceCollection AddUpdatesScraperMongoRepositories(
            this IServiceCollection services,
            MongoDbConfig config)
        {
            return services
                .AddMongoDb(config)
                .AddSingleton<MongoApplicationDbContext>()
                .AddSingleton<IUserLatestUpdateTimesRepository, MongoUserLatestUpdateTimesRepository>()
                .AddSingleton<ISentUpdatesRepository, MongoSentUpdatesRepository>();
        }

        public static IServiceCollection AddUpdatesScraperMockRepositories(
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

        public static IServiceCollection AddUpdatesProducer(
            this IServiceCollection services,
            RabbitMqConfig config)
        {
            return services
                .AddSingleton<IUpdatesProducer>(
                    provider => new UpdatesProducer(
                        config,
                        provider.GetService<ILogger<UpdatesProducer>>()));
        }

        public static IServiceCollection AddUpdatesProvider<TProvider>(
            this IServiceCollection services,
            UpdatesProviderBaseConfig config) where TProvider : class, IUpdatesProvider
        {
            return services
                .AddSingleton(config)
                .AddSingleton<IUpdatesProvider, TProvider>();
        }

        public static IServiceCollection AddVideoExtractor(
            this IServiceCollection services,
            VideoExtractorConfig config)
        {
            return services
                .AddSingleton(config)
                .AddSingleton<VideoExtractor>();
        }

        public static IServiceCollection AddUpdatesScraper(
            this IServiceCollection services,
            ScraperConfig config)
        {
            return services
                .AddSingleton(config)
                .AddSingleton<UpdatesScraper>();
        }

        public static IServiceCollection AddJobsConsumer(
            this IServiceCollection services,
            RabbitMqConfig config)
        {
            return services
                .AddSingleton(config)
                .AddSingleton<IJobsConsumer, JobsConsumer>()
                .AddHostedService(
                    provider => new JobsConsumerService(
                        config,
                        provider.GetService<IJobsConsumer>(),
                        provider.GetService<ILogger<JobsConsumerService>>()));
        }
    }
}