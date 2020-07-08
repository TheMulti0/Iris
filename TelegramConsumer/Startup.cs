using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TelegramConsumer
{
    public static class Startup
    {
        public static Task Main()
        {
            return new HostBuilder()
                .ConfigureAppConfiguration(ConfigureConfiguration)
                .ConfigureLogging(ConfigureLogging)
                .ConfigureServices(ConfigureServices)
                .RunConsoleAsync();
        }

        private static void ConfigureConfiguration(IConfigurationBuilder builder)
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

        private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
        {
            builder
                .AddConfiguration(context.Configuration.GetSection("Logging"))
                .AddCustomConsole();
        }
        
        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            IConfiguration rootConfig = hostContext.Configuration;
            
            var updatesConsumerConfig = rootConfig
                .GetSection("UpdatesConsumer")
                .Get<ConsumerConfig>();

            var telegramConfig = rootConfig
                .GetSection("Telegram")
                .Get<TelegramConfig>();

            services
                .AddConsumer<Unit, Update>(updatesConsumerConfig)
                .AddSingleton(telegramConfig)
                .AddSingleton<ISender, TelegramSender>()
                .AddHostedService<UpdateConsumer>()
                .BuildServiceProvider();
        }
    }
}