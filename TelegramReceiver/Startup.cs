using System;
using System.IO;
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
        .AddSingleton<ICommand, UsersCommand>()
        .AddSingleton<UsersCommand>()
        .AddSingleton<ICommand, SelectPlatformCommand>()
        .AddSingleton<ICommand, AddUserCommand>()
        .AddSingleton<ICommand, SetLanguageCommand>()
        .AddSingleton<ICommand, SetUserDisplayNameCommand>()
        .AddSingleton<ICommand, EnablePrefixCommand>()
        .AddSingleton<ICommand, DisablePrefixCommand>()
        .AddSingleton<ICommand, ManageUserCommand>()
        .AddSingleton<ICommand, SetUserLanguageCommand>()
        .AddSingleton<ICommand, RemoveUserCommand>()
        .AddSingleton<ICommand, ConnectCommand>()
        .AddSingleton<ICommand, DisconnectCommand>()
        .AddSingleton<ICommand, ConnectionCommand>()
        .AddHostedService<MessageHandlerService>()
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();