using System;
using System.Threading.Tasks;
using Common;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.Net;
using Scraper.RabbitMq.Client;
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
            var connectionConfig = rootConfig.GetSection("RabbitMqConnection").Get<RabbitMqConfig>() ?? new();
            var downloaderConfig = rootConfig.GetSection("VideoDownloader").Get<VideoDownloaderConfig>() ?? new();
    
            services
                .AddLanguages()
                .AddSubscriptionsDb()
                .AddScraperRabbitMqClient<NewPostConsumer>(connectionConfig)
                .AddSingleton(provider => ActivatorUtilities.CreateInstance<VideoDownloader>(provider, downloaderConfig))
                .AddSingleton(provider => ActivatorUtilities.CreateInstance<TelegramClientFactory>(provider, telegramConfig))
                .AddSingleton<ISenderFactory, SenderFactory>()
                .AddSingleton<MessageInfoBuilder>()
                .AddSingleton<ITelegramMessageSender, TelegramClientMessageSender>()
                .BuildServiceProvider();
        }
    }
}