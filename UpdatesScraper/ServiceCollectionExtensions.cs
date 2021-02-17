using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                configuration.GetSection<RabbitMqConnectionConfig>("RabbitMqConnection"),
                configuration.GetSection<RabbitMqProducerConfig>("RabbitMqProducer"),
                configuration.GetSection<ScraperConfig>("Scraper"),
                configuration.GetSection<RabbitMqConsumerConfig>("RabbitMqConsumer"));
        }

        public static IServiceCollection AddUpdatesScraper<TProvider>(
            this IServiceCollection services,
            MongoDbConfig mongoDbConfig,
            RabbitMqConnectionConfig connectionConfig,
            RabbitMqProducerConfig producerConfig,
            ScraperConfig scraperConfig,
            RabbitMqConsumerConfig consumerConfig) where TProvider : class, IUpdatesProvider
        {
            services = mongoDbConfig != null 
                ? services.AddUpdatesScraperMongoRepositories(mongoDbConfig) 
                : services.AddUpdatesScraperMockRepositories();

            return services
                .AddRabbitMqConnection(connectionConfig)
                .AddProducer<Update>(producerConfig)
                .AddUpdatesProvider<TProvider>()
                .AddUpdatesScraper(scraperConfig)
                .AddPollJobsConsumer(consumerConfig);
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
            return services
                .AddSingleton<IMongoDbContext>(new MongoDbContext(config.ConnectionString, config.DatabaseName))
                .AddSingleton(config);
        }

        public static IServiceCollection AddUpdatesProvider<TProvider>(
            this IServiceCollection services) where TProvider : class, IUpdatesProvider
        {
            return services
                .AddSingleton<IUpdatesProvider, TProvider>();
        }

        public static IServiceCollection AddUpdatesScraper(
            this IServiceCollection services,
            ScraperConfig config)
        {
            return services
                .AddSingleton(config)
                .AddSingleton<UpdatesScraper>();
        }

        public static IServiceCollection AddPollJobsConsumer(
            this IServiceCollection services,
            RabbitMqConsumerConfig config)
        {
            return services
                .AddSingleton(config)
                .AddSingleton<IConsumer<PollJob>, PollJobsConsumer>()
                .AddConsumerService<PollJob>(config);
        }
    }
}