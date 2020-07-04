using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Consumer;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TelegramConsumer
{
    public static class Startup
    {
        public static Task Main()
        {
            IServiceProvider services = BuildServices();

            services.GetService<Consumer<Unit, Update>>()
                .Messages.Subscribe(
                    result => { Console.WriteLine(result); });

            return Task.Delay(-1);
        }

        private static IServiceProvider BuildServices()
        {
            IConfigurationRoot config = ReadConfiguration();

            var updatesConsumerConfig = config
                .GetSection("UpdatesConsumer")
                .Get<ConsumerConfig>();

            return new ServiceCollection()
                .AddLogging(
                    builder => builder
                        .AddCustomConsole()
                        .AddConfiguration(config))
                .AddConsumer<Unit, Update>(updatesConsumerConfig)
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