using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbGenericRepository;
using Scraper.Net;
using Scraper.RabbitMq.Client;
using Scraper.RabbitMq.Common;
using SharpCompress;
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
                configuration.GetSection<MongoDbConfig>("UpdatesScraperDb"),
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
                .AddScraperRabbitMqClient(config: new RabbitMqConfig
                {
                    ConnectionString = connectionConfig.ConnectionString
                })
                .AddUpdatesScraper(scraperConfig)
                .AddPollJobsConsumer(consumerConfig);
        }

        public static IServiceCollection AddUpdatesScraperMongoRepositories(
            this IServiceCollection services,
            MongoDbConfig config)
        {
            var context = new Lazy<IMongoDbContext>(() => CreateMongoDbContext(config));
            
            return services
                .AddSingleton<IUserLatestUpdateTimesRepository>(_ => new MongoUserLatestUpdateTimesRepository(context.Value))
                .AddSingleton<ISentUpdatesRepository>(_ => new MongoSentUpdatesRepository(context.Value, config));
        }
        
        private static IMongoDbContext CreateMongoDbContext(MongoDbConfig mongoDbConfig)
        {
            return new MongoDbContext(mongoDbConfig.ConnectionString, mongoDbConfig.DatabaseName);
        }

        public static IServiceCollection AddUpdatesScraperMockRepositories(
            this IServiceCollection services)
        {
            return services
                .AddSingleton<IUserLatestUpdateTimesRepository, MockUserLatestUpdateTimesRepository>()
                .AddSingleton<ISentUpdatesRepository, MockSentUpdatesRepository>();
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