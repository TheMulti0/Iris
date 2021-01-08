using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDbGenericRepository;
using TelegramReceiver;
using TelegramReceiver.Data;
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
        .AddJsonFile($"{fileName}.{environmentName}.{fileType}", true); // Overrides default appsettings.json
}

static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
{
    IConfiguration rootConfig = hostContext.Configuration;

    var telegramConfig = rootConfig.GetSection<TelegramConfig>("Telegram");
    var producerConfig = rootConfig.GetSection<RabbitMqConfig>("ChatPollRequestsProducer");
    var mongoConfig = rootConfig.GetSection<MongoDbConfig>("MongoDb");

    services
        .AddLanguages()
        .AddSingleton<IMongoDbContext>(
            _ => new MongoDbContext(
                mongoConfig.ConnectionString,
                mongoConfig.DatabaseName))
        .AddSingleton(mongoConfig)
        .AddSingleton<MongoApplicationDbContext>()
        .AddSingleton<ISavedUsersRepository, MongoSavedUsersRepository>()
        .AddSingleton<TelegramReceiver.Data.MongoApplicationDbContext>()
        .AddSingleton<IConnectionsRepository, MongoConnectionsRepository>()
        .AddProducer<ChatPollRequest>(producerConfig)
        .AddSingleton(telegramConfig)
        .AddSingleton<UsersCommand>();

    Type commandType = typeof(ICommand);
    IEnumerable<Type> commands = commandType.Assembly
        .GetTypes()
        .Where(
            t => t.IsAssignableTo(commandType) && t.IsClass);
    
    foreach (Type command in commands)
    {
        services.AddSingleton(commandType, command);
    }

    services
        .AddSingleton<CommandExecutor>()
        .AddHostedService<MessageHandlerService>()
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();