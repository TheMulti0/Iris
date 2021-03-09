using System;
using System.IO;
using Common;
using Extensions;
using MessagesManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbGenericRepository;
using SubscriptionsDb;

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
    var videoExtractorConfig = rootConfig.GetSection<VideoExtractorConfig>("VideoExtractor");
    var twitterScreenshotterConfig = rootConfig.GetSection<TwitterScreenshotterConfig>("TwitterScreenshotter");

    services
        .AddSubscriptionsDb()

        .AddRabbitMqConnection(connectionConfig)
        .AddProducer<Message>(producerConfig)
        
        .AddSingleton(_ => new VideoExtractor(videoExtractorConfig))
        
        .AddSingleton<IWebDriverFactory>(_ => new WebDriverFactory(twitterScreenshotterConfig))
        .AddSingleton<Screenshotter>()
        
        .AddSingleton<IConsumer<Update>, UpdatesConsumer>()
        .AddConsumerService<Update>(consumerConfig)
        
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();