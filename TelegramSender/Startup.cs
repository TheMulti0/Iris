using System;
using System.Threading.Tasks;
using Common;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PostsListener.Client;
using Scraper.MassTransit.Common;
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

        public static ConfigureServicesResult ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            IConfiguration rootConfig = hostContext.Configuration;

            var telegramConfig = rootConfig.GetSection("Telegram")
                .Get<TelegramClientConfig>();
            var extractorConfig = rootConfig.GetSection("VideoExtractor").Get<VideoExtractorConfig>() ?? new();

            services
                .AddLanguages()
                .AddSubscriptionsDb()
                .AddSingleton(
                    provider => ActivatorUtilities.CreateInstance<HighQualityVideoExtractor>(provider, extractorConfig))
                .AddSingleton(
                    provider => ActivatorUtilities.CreateInstance<TelegramClientFactory>(provider, telegramConfig))
                .AddSingleton<ISenderFactory, SenderFactory>()
                .AddSingleton<MessageInfoBuilder>()
                .AddSingleton<ITelegramMessageSender, TelegramClientMessageSender>();

            return ConfigureServicesResult.MassTransit(x => x.AddPostsListenerClient<NewPostConsumer>());
        }
    }
}