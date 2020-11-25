using System;
using System.IO;
using ConfigProducer;
using Extensions;
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

    builder
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"{fileName}.{fileType}", false)
        .AddJsonFile($"{fileName}.{environmentName}.{fileType}", true); // Overrides default appsettings.json
}

static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
{
    builder
        .AddConfiguration(context.Configuration.GetSection("Logging"))
        .AddCustomConsole();
}

static void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton(GetConfigsReaderConfig)
        .AddSingleton(GetKafkaConfig)
        .AddSingleton<ConfigReader>()
        .AddProducer<string, string>()
        .AddHostedService<ConfigProducer.ConfigProducer>();
}

static ConfigReaderConfig GetConfigsReaderConfig(IServiceProvider provider)
{
    return provider.GetService<IConfiguration>()
        ?.GetSection("ConfigsReader")
        .Get<ConfigReaderConfig>();
}

static BaseKafkaConfig GetKafkaConfig(IServiceProvider provider)
{
    return provider.GetService<IConfiguration>()
        ?.GetSection("Kafka")
        .Get<BaseKafkaConfig>();
}

await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();