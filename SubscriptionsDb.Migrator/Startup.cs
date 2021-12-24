using System;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbGenericRepository;
using Telegram.Bot;
using TelegramClient;
using TelegramReceiver;

namespace SubscriptionsDb.Migrator
{
    public static class Startup
    {
        public static async Task Main()
        {
            await StartupFactory.Run(ConfigureServices);
        }
    
        private static ConfigureServicesResult ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            IConfiguration rootConfig = hostContext.Configuration;

            var mongoConfig = rootConfig.GetSection("ConnectionsDb").Get<MongoDbConfig>();

            services.AddSubscriptionsDb()
                .AddHostedService<SubscriptionsDbMigrator>();

            return ConfigureServicesResult.Empty();
        }

        private static IServiceCollection AddConnectionsDb(
            IServiceCollection services,
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
    }
}