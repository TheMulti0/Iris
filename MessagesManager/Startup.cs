using System;
using System.IO;
using Extensions;
using MessagesManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
    
    var consumerConfig = rootConfig.GetSection<RabbitMqConfig>("MessagesProducer"); 
    var producerConfig = rootConfig.GetSection<RabbitMqConfig>("UpdatesConsumer"); 

    services
        .AddSingleton<IMessagesProducer>(
            _ => new MessagesProducer(producerConfig))
        .AddSingleton<IUpdatesConsumer, UpdatesConsumer>()
        .AddHostedService(
            provider => new UpdatesConsumerService(
                consumerConfig,
                provider.GetService<IUpdatesConsumer>(),
                provider.GetService<ILogger<UpdatesConsumerService>>()))
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();