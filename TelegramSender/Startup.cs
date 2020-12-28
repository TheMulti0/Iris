using System;
using System.IO;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelegramSender;

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

    var telegramConfig = rootConfig.GetSection<TelegramConfig>("Telegram");
    var consumerConfig = rootConfig.GetSection<RabbitMqConfig>("MessagesConsumer");

    services
        .AddLanguages()
        .AddSingleton(telegramConfig)
        .AddSingleton<ISenderFactory, SenderFactory>()
        .AddSingleton<MessageBuilder>()
        .AddSingleton<IConsumer<Message>, MessagesConsumer>()
        .AddConsumerService<Message>(consumerConfig)
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();