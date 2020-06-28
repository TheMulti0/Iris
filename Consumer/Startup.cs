using System;
using System.IO;
using System.Threading.Tasks;
using Confluent.Kafka;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public class Startup
    {
        public Task Main()
        {
            var services = BuildServices();
            
            services.GetService<>()
            
            return Task.Delay(-1);
        }
        
        private static ServiceProvider BuildServices()
        {
            IConfigurationRoot config = ReadConfiguration();

            return new ServiceCollection()
                .AddLogging(
                    builder => builder
                        .AddCustomConsole()
                        .AddConfiguration(config))
                .AddSingleton(
                    config.GetSection("KafkaConsumer").Get<ConsumerConfig>())
                .AddSingleton<KafkaConsumer>()
                .BuildServiceProvider();
        }

        private static IConfigurationRoot ReadConfiguration()
        {
            string environmentName =
                Environment.GetEnvironmentVariable("ENVIRONMENT");

            const string fileName = "appsettings";
            const string fileType = "json";

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{fileName}.{fileType}", false)
                .AddJsonFile($"{fileName}.{environmentName}.{fileType}", true) // Overrides default appsettings.json
                .Build();
        }
    }
}