using System;
using System.Threading.Tasks;
using Common;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.Net;
using Scraper.Net.Facebook;
using Scraper.Net.Feeds;
using Scraper.Net.Twitter;
using Scraper.Net.Youtube;

namespace SingleInstanceApp
{
    public static class Program
    {
        public static async Task Main()
        {
            await StartupFactory.Run(
                ConfigureScraper,
                ConfigurePostsListener,
                TelegramSender.Startup.ConfigureServices,
                TelegramReceiver.Startup.ConfigureServices);
        }

        private static ConfigureServicesResult ConfigureScraper(HostBuilderContext context, IServiceCollection services)
        {
            services.AddScraper(builder => BuildScraper(builder, context.Configuration));
            
            return ConfigureServicesResult.Empty();
        }
        
        private static void BuildScraper(ScraperBuilder builder, IConfiguration configuration)
        {
            IConfiguration scraperConfig = configuration.GetSection("Scraper");
            
            IConfigurationSection feedsConfig = scraperConfig.GetSection("Feeds");
            if (feedsConfig.GetValue<bool?>("Enabled") != false)
            {
                builder.AddFeeds();
            }

            IConfigurationSection twitterConfig = scraperConfig.GetSection("Twitter");
            var twitterConfigg = twitterConfig.Get<TwitterConfig>();
            if (twitterConfig.GetValue<bool>("Enabled") && twitterConfigg != null)
            {
                builder.AddTwitter(twitterConfigg);
            }

            IConfigurationSection facebookConfig = scraperConfig.GetSection("Facebook");
            if (facebookConfig.GetValue<bool>("Enabled"))
            {
                builder.AddFacebook(facebookConfig.Get<FacebookConfig>());
            }
            
            IConfigurationSection youtubeConfig = scraperConfig.GetSection("Youtube");
            if (youtubeConfig.GetValue<bool>("Enabled"))
            {
                builder.AddYoutube(youtubeConfig.Get<YoutubeConfig>());
            }
        }

        private static ConfigureServicesResult ConfigurePostsListener(HostBuilderContext context, IServiceCollection services)
        {
            Action<IServiceCollectionBusConfigurator> callback = new PostsListener.Startup(context.Configuration).ConfigureServices(services);

            return ConfigureServicesResult.MassTransit(callback);
        }
    }
}