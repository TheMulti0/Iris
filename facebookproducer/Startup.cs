using System;
using System.IO;
using Extensions;
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
    
    var mongoDbSettings = rootConfig
            .GetSection("MongoDb")
            ?.Get<MongoDbConfig>();

    services
        .AddMongoDb(mongoDbSettings)
        .AddSingleton<ApplicationDbContext>()
        .AddSingleton<IUserLatestUpdateTimesRepository, UserLatestUpdateTimesRepository>()
        .BuildServiceProvider();
}
    
await new HostBuilder()
    .ConfigureAppConfiguration(ConfigureConfiguration)
    .ConfigureLogging(ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();