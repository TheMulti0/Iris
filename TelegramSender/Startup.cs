using System.Threading.Tasks;
using Common;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    
            var telegramConfig = rootConfig.GetSection("Telegram").Get<RabbitMqConfig>();
            var connectionConfig = rootConfig.GetSection("RabbitMqConnection").Get<RabbitMqConfig>();
    
            services
                .AddSubscriptionsDb()
                .AddLanguages()
                .AddMassTransit(
                    x =>
                    {
                        x.AddConsumer<MessageConsumer>();
                        
                        x.UsingRabbitMq(
                            (context, cfg) =>
                            {
                                cfg.Host(connectionConfig.ConnectionString);
                                
                                cfg.ConfigureJsonDeserializer(JsonConfigurator.Configure);
                                
                                cfg.ConfigureEndpoints(context);
                            });
                    })
                .AddMassTransitHostedService()
                .AddSingleton(telegramConfig)
                .AddSingleton<TelegramClientFactory>()
                .AddSingleton<ISenderFactory, SenderFactory>()
                .AddSingleton<MessageInfoBuilder>()
                .BuildServiceProvider();
        }
    }
}