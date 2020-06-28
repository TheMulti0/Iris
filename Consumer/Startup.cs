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
        public static Task Main()
        {
            var services = BuildServices();

            services.GetService<TopicConsumer<Ignore, Update>>().Messages.Subscribe(
                result =>
                {
                    Console.WriteLine(result.Message.Value.Content);
                });
            
            return Task.Delay(-1);
        }
        
        private static IServiceProvider BuildServices()
        {
            IConfigurationRoot config = ReadConfiguration();

            IServiceCollection services = new ServiceCollection()
                .AddLogging(
                    builder => builder
                        .AddCustomConsole()
                        .AddConfiguration(config));

            AddTopicConsumer<Ignore, Update>(
                services,
                config.GetSection("UpdatesConsumer"));
            
            return services
                .BuildServiceProvider();
        }

        private static void AddTopicConsumer<TKey, TValue>(
            IServiceCollection services,
            IConfiguration config)
        {
            var consumerConfig = config.Get<TopicConsumerConfig>();

            services.AddSingleton(_ => new TopicConsumer<TKey, TValue>(consumerConfig));
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