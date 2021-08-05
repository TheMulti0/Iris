using System;
using System.IO;
using Common;
using Extensions;
using HtmlCssToImage.Net;
using MessagesManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.RabbitMq.Client;
using SubscriptionsDb;
using UpdatesDb;

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
    var clientConfig = rootConfig.GetSection<ScraperRabbitMqClientConfig>("ScraperRabbitMqClient");

    services
        .AddSubscriptionsDb()
        .AddUpdatesDb()

        .AddRabbitMqConnection(connectionConfig)
        .AddProducer<Message>(producerConfig)
        
        .AddSingleton<IConsumer<Update>, UpdatesConsumer>()
        .AddConsumerService<Update>(consumerConfig)
        
        .AddScraperRabbitMqClient(clientConfig.ServerUri, connectionConfig.ConnectionString)
        
        .AddHostedService<NewPostsConsumer>()
        
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();