using System;
using System.IO;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        .AddJsonFile($"{fileName}.{environmentName}.{fileType}", true); // Overrides default appsettings.json
}

static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
{
    IConfiguration rootConfig = hostContext.Configuration;
    
    var consumerConfig = rootConfig.GetSection<RabbitMqConfig>("PollRequestsConsumer"); 
    var producerConfig = rootConfig.GetSection<RabbitMqConfig>("JobsProducer"); 
    var pollerConfig = rootConfig.GetSection<SubscriptionsPollerConfig>("SubscriptionsPoller"); 

    services
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