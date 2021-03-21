using System;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbGenericRepository;

namespace UpdatesDb
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddUpdatesDb(
            this IServiceCollection services,
            IMongoDbContext dbContext = null)
        {
            IConfiguration config = null;
            
            var context = new Lazy<IMongoDbContext>(() => dbContext ?? CreateMongoDbContext(GetMongoDbConfig(config)));
            
            return services
                .AddSingleton<IUpdatesRepository>(provider =>
                {
                    config = provider.GetService<IConfiguration>();
                    
                    return new MongoUpdatesRepository(context.Value, GetMongoDbConfig(config));
                })
                .AddSingleton<IFeedsRepository>(_ => new MongoFeedsRepository(context.Value));
        }

        private static MongoDbConfig GetMongoDbConfig(IConfiguration config)
        {
            return config.GetSection<MongoDbConfig>("UpdatesDb");
        }

        private static IMongoDbContext CreateMongoDbContext(MongoDbConfig mongoDbConfig)
        {
            return new MongoDbContext(mongoDbConfig.ConnectionString, mongoDbConfig.DatabaseName);
        }
    }
}