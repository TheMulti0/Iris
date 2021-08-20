using System;
using System.IO;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbGenericRepository;
using Scraper.RabbitMq.Client;
using Scraper.RabbitMq.Common;
using TelegramReceiver;
using SubscriptionsDb;

static void ConfigureConfiguration(IConfigurationBuilder builder)
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

static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
{
    IConfiguration rootConfig = hostContext.Configuration;

    var mongoConfig = rootConfig.GetSection<MongoDbConfig>("ConnectionsDb");
    var connectionConfig = rootConfig.GetSection<RabbitMqConnectionConfig>("RabbitMqConnection");
    var producerConfig = rootConfig.GetSection<RabbitMqProducerConfig>("RabbitMqProducer");
    var telegramConfig = rootConfig.GetSection<TelegramConfig>("Telegram");

    AddMongoDbRepositories(services, mongoConfig);
    AddRabbitMq(services, connectionConfig, producerConfig);
    AddValidators(services);
    AddCommandHandling(services, telegramConfig);
    
    services
        .AddSingleton<ISubscriptionsManager, NewSubscriptionsManager>()
        .AddScraperRabbitMqClient(config: new RabbitMqConfig
        {
            ConnectionString = connectionConfig.ConnectionString
        })
        .AddLanguages()
        .BuildServiceProvider();
}

static IServiceCollection AddMongoDbRepositories(
    IServiceCollection services,
    MongoDbConfig config)
{
    return AddReceiverMongoRepositories(
        services.AddSubscriptionsDb(),
        config);
}

static IServiceCollection AddReceiverMongoRepositories(
    IServiceCollection services,
    MongoDbConfig config)
{
    var context = new Lazy<IMongoDbContext>(() => CreateMongoDbContext(config));
        
    return services
        .AddSingleton<IConnectionsRepository>(_ => new MongoConnectionsRepository(context.Value));
}
    
static IMongoDbContext CreateMongoDbContext(MongoDbConfig mongoDbConfig)
{
    return new MongoDbContext(mongoDbConfig.ConnectionString, mongoDbConfig.DatabaseName);
}

static IServiceCollection AddRabbitMq(
    IServiceCollection services, 
    RabbitMqConnectionConfig connectionConfig,
    RabbitMqProducerConfig producerConfig)
{
    return services
        .AddRabbitMqConnection(connectionConfig)
        .AddProducer<ChatSubscriptionRequest>(producerConfig);
}

static IServiceCollection AddValidators(
    IServiceCollection services)
{
    return services
        .AddSingleton<FacebookValidator>()
        .AddSingleton<TwitterValidator>()
        .AddSingleton(
            provider => new UserValidator(
                provider.GetService<FacebookValidator>(),
                provider.GetService<TwitterValidator>()));
}

static IServiceCollection AddCommandHandling(
    IServiceCollection services, 
    TelegramConfig telegramConfig)
{
    return services
        .AddSingleton(telegramConfig)
        .AddSingleton<CommandFactory>()
        .AddSingleton<CommandExecutor>()
        .AddHostedService<MessageHandlerService>();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();