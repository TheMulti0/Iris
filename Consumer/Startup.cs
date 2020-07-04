using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
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

            services.GetService<Consumer<Unit, Update>>().Messages.Subscribe(
                result =>
                {
                    Console.WriteLine(result);
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

            AddTopicConsumer<Unit, Update>(
                services,
                config.GetSection("UpdatesConsumer"));
            
            return services
                .BuildServiceProvider();
        }

        private static void AddTopicConsumer<TKey, TValue>(
            IServiceCollection services,
            IConfiguration config)
        {
            services.AddSingleton(config.Get<ConsumerConfig>());

            services.AddSingleton(s => new Consumer<TKey, TValue>(s.GetService<ConsumerConfig>()));
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