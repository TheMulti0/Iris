using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using TheMulti0.Console;

namespace Common
{
    public static class StartupFactory
    {
        public static async Task Run(Action<HostBuilderContext, IServiceCollection> configureServices)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(ConfigureHostConfiguration)
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureLogging(ConfigureLogging)
                .ConfigureServices(configureServices)
                .RunConsoleAsync();
        }

        private static void ConfigureHostConfiguration(IConfigurationBuilder builder)
        {
            builder.AddEnvironmentVariables();
        }

        private static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
            IHostEnvironment env = context.HostingEnvironment;

            builder
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();
        }

        private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            builder.AddTheMulti0Console();
            builder.AddSentry();
        }
    }
}