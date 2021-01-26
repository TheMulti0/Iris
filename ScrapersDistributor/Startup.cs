using System;
using System.IO;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    var producerConfig = rootConfig.GetSection<RabbitMqProducerConfig>("RabbitMqProducer"); 
    var pollerConfig = rootConfig.GetSection<SubscriptionsPollerConfig>("SubscriptionsPoller"); 

    services
        .AddRabbitMqConnection(connectionConfig)
        .AddProducer<User>(producerConfig)
        .AddSingleton<IConsumer<SubscriptionRequest>, SubscriptionsConsumer>()
        .AddConsumerService<SubscriptionRequest>(consumerConfig)
        .AddSingleton(pollerConfig)
        .AddSingleton<ISubscriptionsManagerClient, SubscriptionsManagerClient>()
        .AddHostedService<SubscriptionsPollerService>()
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();