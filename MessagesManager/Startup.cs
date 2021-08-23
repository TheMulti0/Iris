using System;
using System.IO;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.RabbitMq.Client;
using Scraper.RabbitMq.Common;
using SubscriptionsDb;

namespace MessagesManager
{
    public static class Startup
    {
        public static async Task Main()
        {
            await new HostBuilder()
                .ConfigureAppConfiguration(ConfigureConfiguration)
                .ConfigureLogging(CustomConsoleDiExtensions.ConfigureLogging)
                .ConfigureServices(ConfigureServices)
                .RunConsoleAsync();
        }
    
        private static void ConfigureConfiguration(IConfigurationBuilder builder)
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

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            IConfiguration rootConfig = hostContext.Configuration;

            var connectionConfig = rootConfig.GetSection<RabbitMqConnectionConfig>("RabbitMqConnection"); 
            var producerConfig = rootConfig.GetSection<RabbitMqProducerConfig>("RabbitMqProducer");

            services
                .AddSubscriptionsDb()

                .AddRabbitMqConnection(connectionConfig)
                .AddProducer<Message>(producerConfig)
        
                .AddSingleton<IConsumer<Update>, UpdatesConsumer>()
                .AddScraperRabbitMqClient<NewPostConsumer>(new RabbitMqConfig
                {
                    ConnectionString = connectionConfig.ConnectionString
                })
        
                .BuildServiceProvider();
        }   
    }
}