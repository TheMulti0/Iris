using System;
using System.IO;
using Common;
using Extensions;
using FacebookScraper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbGenericRepository;
using TelegramReceiver;
using TwitterScraper;
using UserDataLayer;
using MongoApplicationDbContext = UserDataLayer.MongoApplicationDbContext;

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

    var mongoConfig = rootConfig.GetSection<MongoDbConfig>("MongoDb");
    var connectionConfig = rootConfig.GetSection<RabbitMqConnectionConfig>("RabbitMqConnection");
    var producerConfig = rootConfig.GetSection<RabbitMqProducerConfig>("RabbitMqProducer");
    var telegramConfig = rootConfig.GetSection<TelegramConfig>("Telegram");
    var twitterConfig = rootConfig.GetSection<TwitterUpdatesProviderConfig>("Twitter");

    AddMongoDbRepositories(services, mongoConfig);
    AddRabbitMq(services, connectionConfig, producerConfig);
    AddValidators(services, twitterConfig);
    AddCommandHandling(services, telegramConfig);
    
    services
        .AddLanguages()
        .BuildServiceProvider();
}

static IServiceCollection AddMongoDbRepositories(
    IServiceCollection services,
    MongoDbConfig config)
{
    return services.AddSingleton<IMongoDbContext>(
        _ => new MongoDbContext(
            config.ConnectionString,
            config.DatabaseName))
        .AddSingleton(config)
        .AddSingleton<MongoApplicationDbContext>()
        .AddSingleton<ISavedUsersRepository, MongoSavedUsersRepository>()
        .AddSingleton<TelegramReceiver.MongoApplicationDbContext>()
        .AddSingleton<IConnectionsRepository, MongoConnectionsRepository>();
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
    IServiceCollection services,
    TwitterUpdatesProviderConfig twitterConfig)
{
    return services
        .AddSingleton<FacebookUpdatesProvider>()
        .AddSingleton<FacebookValidator>()
        .AddSingleton(
            _ => new TwitterUpdatesProvider(twitterConfig))
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