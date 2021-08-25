using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.RabbitMq.Client;
using Scraper.RabbitMq.Common;
using SubscriptionsDb;

namespace MessagesManager
{
    public static class Startup
    {
        public static async Task Main()
        {
            await StartupFactory.Run(ConfigureServices);
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            IConfiguration rootConfig = hostContext.Configuration;

            var connectionConfig = rootConfig.GetSection("RabbitMqConnection").Get<RabbitMqConfig>(); 

            services
                .AddSubscriptionsDb()
                .AddScraperRabbitMqClient(typeof(NewPostConsumer), connectionConfig)
                .BuildServiceProvider();
        }   
    }
}