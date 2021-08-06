﻿using System;
using System.IO;
using Common;
using Extensions;
using MessagesManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.RabbitMq.Client;
using SubscriptionsDb;
using UpdatesDb;
using RabbitMqConsumerConfig = Scraper.RabbitMq.Client.RabbitMqConsumerConfig;

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
    var producerConfig = rootConfig.GetSection<RabbitMqProducerConfig>("RabbitMqProducer");
    var clientConfig = rootConfig.GetSection<ScraperRabbitMqClientConfig>("ScraperRabbitMqClient");
    var consumerConfig = rootConfig.GetSection<RabbitMqConsumerConfig>("RabbitMqConsumer");
    consumerConfig.ConnectionString = connectionConfig.ConnectionString;

    services
        .AddSubscriptionsDb()
        .AddUpdatesDb()

        .AddRabbitMqConnection(connectionConfig)
        .AddProducer<Message>(producerConfig)
        
        .AddSingleton<IConsumer<Update>, UpdatesConsumer>()
        
        .AddScraperRabbitMqClient(clientConfig.ServerUri, consumerConfig)
        
        .AddHostedService<NewPostsConsumer>()
        
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();