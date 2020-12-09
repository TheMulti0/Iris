using System;
using System.IO;
using System.Text.Json;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelegramBot;
using UpdatesConsumer;

static void ConfigureConfiguration(IConfigurationBuilder builder)
{
    string environmentName =
        Environment.GetEnvironmentVariable("ENVIRONMENT");

    const string fileName = "appsettings";
    const string fileType = "json";

    builder
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"{fileName}.{fileType}", false)
        .AddJsonFile($"{fileName}.{environmentName}.{fileType}", true); // Overrides default appsettings.json
}

static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
{
    builder
        .AddConfiguration(context.Configuration)
        .AddCustomConsole();

    if (context.Configuration.GetSection("Sentry")
        .Exists())
    {
        builder.AddSentry();
    }
}

static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
{
    IConfiguration rootConfig = hostContext.Configuration;

    var updatesConsumerConfig = rootConfig
        .GetSection("UpdatesConsumer")
        ?
        .Get<RabbitMqConfig>();

    var defaultTelegramConfig = rootConfig
        .GetSection("Telegram")
        ?.Get<TelegramConfig>();

    services
        .AddSingleton(updatesConsumerConfig)
        .AddSingleton<RabbitMqConsumer>()
        .AddSingleton(defaultTelegramConfig)
        .AddSingleton<IConfigProvider, ConfigProvider>()
        .AddSingleton<ITelegramBotClientProvider, TelegramBotClientProvider>()
        .AddSingleton<MessageBuilder>()
        .AddSingleton<IUpdateConsumer, TelegramBot.TelegramBot>()
        .AddHostedService<UpdatesConsumerService>()
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();