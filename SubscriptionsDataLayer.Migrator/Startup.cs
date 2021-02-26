using System;
using System.IO;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbGenericRepository;
using SubscriptionsDataLayer;
using SubscriptionsDataLayer.Migrator;
using MongoApplicationDbContext = SubscriptionsDataLayer.MongoApplicationDbContext;

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

    var mongoConfig = rootConfig.GetSection<MongoDbConfig>("MongoDb");

    services
        .AddSingleton<IMongoDbContext>(
            _ => new MongoDbContext(mongoConfig.ConnectionString, mongoConfig.DatabaseName))
        .AddSingleton(mongoConfig)
        .AddSingleton<MongoApplicationDbContext>()
        .AddSingleton<IChatSubscriptionsRepository, MongoChatSubscriptionsRepository>()
        .AddLanguages()
        .AddHostedService<PrefixSuffixMigrator>()
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
        .AddSingleton<IChatSubscriptionsRepository, MongoChatSubscriptionsRepository>();
}

await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();