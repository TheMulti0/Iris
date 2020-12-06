using System;
using System.IO;
using Extensions;
using FacebookProducer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UpdatesProducer;

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
    
    var mongoDbConfig = rootConfig
            .GetSection("MongoDb")
            ?.Get<MongoDbConfig>();
            
    var kafkaConfig = rootConfig
            .GetSection("Kafka")
            ?.Get<BaseKafkaConfig>();
            
    var pollerConfig = rootConfig
            .GetSection("Poller")
            ?.Get<PollerConfig>();

    services
        .AddUpdatesProducer<FacebookUpdatesProvider>(
            mongoDbConfig, kafkaConfig, pollerConfig)
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();