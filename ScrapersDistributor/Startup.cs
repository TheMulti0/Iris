using System;
using System.IO;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.RabbitMq.Client;
using ScrapersDistributor;

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
    
    var connectionConfig = rootConfig.GetSection<RabbitMqConnectionConfig>("RabbitMqConnection");
    var consumerConfig = rootConfig.GetSection<RabbitMqConsumerConfig>("RabbitMqConsumer");
    var clientConfig = rootConfig.GetSection<ScraperRabbitMqClientConfig>("ScraperRabbitMqClient");

    services
        .AddRabbitMqConnection(connectionConfig)
        .AddSingleton<IConsumer<SubscriptionRequest>, SubscriptionRequestsConsumer>()
        .AddConsumerService<SubscriptionRequest>(consumerConfig)
        .AddScraperRabbitMqClient(clientConfig.ServerUri, connectionConfig.ConnectionString)
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();