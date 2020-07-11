using System;
using System.IO;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConfigProducer
{
    public class Startup
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
        
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(GetConfigsReaderConfig)
                .AddSingleton(GetKafkaConfig)
                .AddSingleton<ConfigReader>()
                .AddProducer<string, string>()
                .AddHostedService<ConfigProducer>();
        }
        
        private static ConfigReaderConfig GetConfigsReaderConfig(IServiceProvider provider)
        {
            return provider.GetService<IConfiguration>().GetSection("ConfigsReader")
                .Get<ConfigReaderConfig>();
        }
    
        private static BaseKafkaConfig GetKafkaConfig(IServiceProvider provider)
        {
            return provider.GetService<IConfiguration>().GetSection("Kafka")
                .Get<BaseKafkaConfig>();
        }
    }
}