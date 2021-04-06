using System;
using System.IO;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbGenericRepository;
using SubscriptionsDb;
using SubscriptionsDb.Migrator;
using Telegram.Bot;
using TelegramClient;
using TelegramReceiver;

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
        .AddEnvironmentVariables();
}

static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
{
    IConfiguration rootConfig = hostContext.Configuration;

    var mongoConfig = rootConfig.GetSection<MongoDbConfig>("ConnectionsDb");
    var telegramConfig = rootConfig.GetSection<TelegramClientConfig>("Telegram");

    AddConnectionsDb(services, mongoConfig)
        .AddSubscriptionsDb()
        .AddSingleton(new TelegramBotClient(telegramConfig.BotToken))
        .AddHostedService<SubscriptionsDbMigrator>()
        .AddHostedService<ConnectionsDbMigrator>()
        .BuildServiceProvider();
}

    static IServiceCollection AddConnectionsDb(
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
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();