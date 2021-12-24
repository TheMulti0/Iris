using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scraper.MassTransit.Client;
using Scraper.MassTransit.Common;
using Scraper.Net;
using TheMulti0.Console;

namespace Common
{
    public static class StartupFactory
    {
        public static async Task Run(params ConfigureServices[] configureServices)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(ConfigureHostConfiguration)
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureLogging(ConfigureLogging)
                .ConfigureServices((hostContext, services) =>
                {
                    IEnumerable<ConfigureServicesResult> results = configureServices.Select(c => c(hostContext, services));
                    
                    var massTransitResults = results.Where(result => result.AddMassTransit).ToArray();

                    if (massTransitResults.Any())
                    {
                        AddMassTransit(
                            hostContext,
                            services,
                            massTransitResults.Select(result => result.MassTransitCallback).ToArray());    
                    }
                    
                })
                .RunConsoleAsync();
        }

        public static IServiceCollection AddMassTransit(
            HostBuilderContext hostContext,
            IServiceCollection services,
            params Action<IServiceCollectionBusConfigurator>[] configure)
        {
            var config = hostContext.Configuration.GetSection("RabbitMqConnection").Get<RabbitMqConfig>();

            void InMemory(IBusRegistrationContext context, IInMemoryBusFactoryConfigurator cfg)
            {
                cfg.ConfigureInterfaceJsonSerialization(typeof(IMediaItem));
                cfg.ConfigureEndpoints(context);
            }
            
            void RabbitMq(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
            {
                cfg.Host(config.ConnectionString);

                cfg.ConfigureInterfaceJsonSerialization(typeof(IMediaItem));
                cfg.ConfigureEndpoints(context);
            }
            
            void Configure(IServiceCollectionBusConfigurator x)
            {
                foreach (Action<IServiceCollectionBusConfigurator> action in configure)
                {
                    action(x);
                }
                

                if (config == null)
                {
                    x.UsingInMemory(InMemory);
                }
                else
                {
                    services.AddScraperMassTransitClient();
                    x.UsingRabbitMq(RabbitMq);
                }
            }

            return services
                .AddMassTransit(Configure)
                .AddMassTransitHostedService();
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
            builder.AddConfiguration(context.Configuration);
            builder.AddTheMulti0Console();
            builder.AddSentry();
        }
    }
}