using System;
using System.IO;
using Common;
using Extensions;
using MessagesManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDbGenericRepository;
using UserDataLayer;

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
        .AddJsonFile($"{fileName}.{environmentName}.{fileType}", true); // Overrides default appsettings.json
}

static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
{
    IConfiguration rootConfig = hostContext.Configuration;
    
    var consumerConfig = rootConfig.GetSection<RabbitMqConfig>("UpdatesConsumer"); 
    var producerConfig = rootConfig.GetSection<RabbitMqConfig>("MessagesProducer");
    var mongoConfig = rootConfig.GetSection<MongoDbConfig>("MongoDb");

    services
        .AddSingleton<IMongoDbContext>(
            _ => new MongoDbContext(
                mongoConfig.ConnectionString,
                mongoConfig.DatabaseName))
        .AddSingleton(mongoConfig)
        .AddSingleton<MongoApplicationDbContext>()
        .AddSingleton<ISavedUsersRepository, MongoSavedUsersRepository>()
        .AddProducer<Message>(producerConfig)
        .AddSingleton<IConsumer<Update>, UpdatesConsumer>()
        .AddConsumerService<Update>(consumerConfig)
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();