using System;
using Common;
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
            
            var context = new Lazy<IMongoDbContext>(() => dbContext ?? CreateMongoDbContext(config));
            
            return services
                .AddSingleton<IUpdatesRepository>(provider =>
                {
                    config = provider.GetService<IConfiguration>();
                    
                    return new MongoUpdatesRepository(context.Value);
                });
        }

        private static IMongoDbContext CreateMongoDbContext(IConfiguration config)
        {
            var mongoDbConfig = config.GetSection<MongoDbConfig>("UpdatesDb");

            return new MongoDbContext(mongoDbConfig.ConnectionString, mongoDbConfig.DatabaseName);
        }
    }
}