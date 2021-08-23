using System;
using System.IO;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbGenericRepository;
using Scraper.RabbitMq.Client;
using Scraper.RabbitMq.Common;
using SubscriptionsDb;

namespace TelegramReceiver
{
    public static class Startup
    {
        public static async Task Main()
        {
            await new HostBuilder()
                .ConfigureAppConfiguration(ConfigureConfiguration)
                .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
                .ConfigureServices(ConfigureServices)
                .RunConsoleAsync();
        }

        private static void ConfigureConfiguration(IConfigurationBuilder builder)
        {
            string environmentName =
                Environment.GetEnvironmentVariable("ENVIRONMENT");

            const string fileName = "appsettings";
            const string fileType = "json";

            string basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                Environment.GetEnvironmentVariable("CONFIG_DIRECTORY") ?? string.Empty);

            builder
                .SetBasePath(basePath)
                .AddJsonFile($"{fileName}.{fileType}", false)
                .AddJsonFile($"{fileName}.{environmentName}.{fileType}", true) // Overrides default appsettings.json
                .AddUserSecrets<TelegramConfig>()
                .AddEnvironmentVariables();
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            IConfiguration rootConfig = hostContext.Configuration;

            var mongoConfig = rootConfig.GetSection<MongoDbConfig>("ConnectionsDb");
            var connectionConfig = rootConfig.GetSection<RabbitMqConnectionConfig>("RabbitMqConnection");
            var producerConfig = rootConfig.GetSection<RabbitMqProducerConfig>("RabbitMqProducer");
            var telegramConfig = rootConfig.GetSection<TelegramConfig>("Telegram");

            services
                .AddMongoDbRepositories(mongoConfig)
                .AddRabbitMq(connectionConfig, producerConfig)
                .AddValidators()
                .AddCommandHandling(telegramConfig)
                .AddSingleton<ISubscriptionsManager, NewSubscriptionsManager>()
                .AddScraperRabbitMqClient(
                    config: new RabbitMqConfig
                    {
                        ConnectionString = connectionConfig.ConnectionString
                    })
                .AddLanguages()
                .BuildServiceProvider();
        }

        private static IServiceCollection AddMongoDbRepositories(
            this IServiceCollection services,
            MongoDbConfig config)
        {
            return AddReceiverMongoRepositories(
                services.AddSubscriptionsDb(),
                config);
        }

        private static IServiceCollection AddReceiverMongoRepositories(
            this IServiceCollection services,
            MongoDbConfig config)
        {
            var context = new Lazy<IMongoDbContext>(() => CreateMongoDbContext(config));

            return services
                .AddSingleton<IConnectionsRepository>(_ => new MongoConnectionsRepository(context.Value));
        }

        private static IMongoDbContext CreateMongoDbContext(MongoDbConfig mongoDbConfig)
        {
            return new MongoDbContext(mongoDbConfig.ConnectionString, mongoDbConfig.DatabaseName);
        }

        private static IServiceCollection AddRabbitMq(
            this IServiceCollection services,
            RabbitMqConnectionConfig connectionConfig,
            RabbitMqProducerConfig producerConfig)
        {
            return services
                .AddRabbitMqConnection(connectionConfig)
                .AddProducer<ChatSubscriptionRequest>(producerConfig);
        }

        private static IServiceCollection AddValidators(this IServiceCollection services)
        {
            return services
                .AddSingleton<FacebookValidator>()
                .AddSingleton<TwitterValidator>()
                .AddSingleton(
                    provider => new UserValidator(
                        provider.GetService<FacebookValidator>(),
                        provider.GetService<TwitterValidator>()));
        }

        private static IServiceCollection AddCommandHandling(
            this IServiceCollection services,
            TelegramConfig telegramConfig)
        {
            return services
                .AddSingleton(telegramConfig)
                .AddSingleton<CommandFactory>()
                .AddSingleton<CommandExecutor>()
                .AddHostedService<MessageHandlerService>();
        }
    }
}