using System;
using System.Threading.Tasks;
using Common;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbGenericRepository;
using PostsListener.Client;
using Scraper.MassTransit.Client;
using Scraper.MassTransit.Common;
using Scraper.Net;
using SubscriptionsDb;

namespace TelegramReceiver
{
    public static class Startup
    {
        public static async Task Main()
        {
            await StartupFactory.Run(ConfigureServices);
        }

        public static ConfigureServicesResult ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            IConfiguration rootConfig = hostContext.Configuration;

            var mongoConfig = rootConfig.GetSection("ConnectionsDb").Get<MongoDbConfig>();
            var telegramConfig = rootConfig.GetSection("Telegram").Get<TelegramConfig>();

            services
                .AddMongoDbRepositories(mongoConfig)
                .AddSingleton(provider => new UserValidator(provider.GetRequiredService<IScraperService>(), rootConfig.GetValue<bool>("ValidateUsers")))
                .AddCommandHandling(telegramConfig)
                .AddSingleton<ISubscriptionsManager>(
                    provider => new SubscriptionsManager(
                        provider.GetRequiredService<INewPostSubscriptionsClient>(),
                        rootConfig.GetValue<bool>("SubscribeToOldPosts")))
                .AddLanguages();

            if (rootConfig.GetSection("SubscriptionsUpdater").GetValue<bool>("IsEnabled"))
            {
                services.AddHostedService<SubscriptionsUpdater>();
            }

            return ConfigureServicesResult.MassTransit(x => x.AddPostsListenerClient());
        }

        private static IServiceCollection AddMongoDbRepositories(
            this IServiceCollection services,
            MongoDbConfig config)
        {
            return AddReceiverMongoRepositories(
                services.AddSubscriptionsDb(),
                config);
        }

        private static IServiceCollection AddReceiverMongoRepositories(
            this IServiceCollection services,
            MongoDbConfig config)
        {
            var context = new Lazy<IMongoDbContext>(() => CreateMongoDbContext(config));

            return services
                .AddSingleton<IConnectionsRepository>(_ => new MongoConnectionsRepository(context.Value));
        }

        private static IMongoDbContext CreateMongoDbContext(MongoDbConfig mongoDbConfig)
        {
            return new MongoDbContext(mongoDbConfig.ConnectionString, mongoDbConfig.DatabaseName);
        }

        private static IServiceCollection AddCommandHandling(
            this IServiceCollection services,
            TelegramConfig telegramConfig)
        {
            return services
                .AddSingleton(telegramConfig)
                .AddSingleton<CommandFactory>()
                .AddSingleton<CommandExecutor>()
                .AddHostedService<MessageHandlerService>();
        }
    }
}