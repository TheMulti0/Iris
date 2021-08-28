using System;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbGenericRepository;

namespace SubscriptionsDb
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddSubscriptionsDb(
            this IServiceCollection services,
            IMongoDbContext dbContext = null)
        {
            IConfiguration config = null;
            
            var context = new Lazy<IMongoDbContext>(() => dbContext ?? CreateMongoDbContext(config));
            
            return services
                .AddSingleton(context.Value)
                .AddSingleton<IChatSubscriptionsRepository>(provider =>
                {
                    config = provider.GetService<IConfiguration>();
                    
                    return new MongoChatSubscriptionsRepository(context.Value);
                });
        }

        private static IMongoDbContext CreateMongoDbContext(IConfiguration config)
        {
            var mongoDbConfig = config.GetSection("SubscriptionsDb").Get<MongoDbConfig>();

            return new MongoDbContext(mongoDbConfig.ConnectionString, mongoDbConfig.DatabaseName);
        }
    }
}