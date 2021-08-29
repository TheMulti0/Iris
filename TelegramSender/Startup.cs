using System;
using System.Threading.Tasks;
using Common;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.Net;
using Scraper.RabbitMq.Common;
using SubscriptionsDb;
using TelegramClient;

namespace TelegramSender
{
    public class Startup
    {
        public static async Task Main()
        {
            await StartupFactory.Run(ConfigureServices);
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            IConfiguration rootConfig = hostContext.Configuration;

            var telegramConfig = rootConfig.GetSection("Telegram")
                .Get<TelegramClientConfig>();
            var connectionConfig = rootConfig.GetSection("RabbitMqConnection").Get<RabbitMqConfig>();
            var downloaderConfig = rootConfig.GetSection("VideoDownloader").Get<VideoDownloaderConfig>();
    
            services
                .AddLogging()
                .AddSubscriptionsDb()
                .AddLanguages()
                .AddMassTransit(
                    x =>
                    {
                        x.AddConsumer<MessageConsumer>(
                            c =>
                            {
                                c.UseMessageRetry(
                                    r => r.Interval(3, TimeSpan.FromSeconds(5)));
                                c.UseScheduledRedelivery(
                                    r => r.None());
                            });
                        
                        x.UsingRabbitMq(
                            (context, cfg) =>
                            {
                                cfg.Host(connectionConfig.ConnectionString);
                                
                                cfg.ConfigureInterfaceJsonSerialization();
                                
                                cfg.ConfigureEndpoints(context);
                            });
                    })
                .AddMassTransitHostedService()
                .AddSingleton(provider => ActivatorUtilities.CreateInstance<VideoDownloader>(provider, downloaderConfig))
                .AddSingleton(provider => ActivatorUtilities.CreateInstance<TelegramClientFactory>(provider, telegramConfig))
                .AddSingleton<ISenderFactory, SenderFactory>()
                .AddSingleton<MessageInfoBuilder>()
                .BuildServiceProvider();
        }
    }
}