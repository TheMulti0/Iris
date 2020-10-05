using System.Threading.Tasks;
using Dashboard.Data;
using Microsoft.Extensions.DependencyInjection;
using UpdatesConsumer;

namespace Dashboard.Controllers
{
    public class UpdatesDataLayerAppender : IUpdateConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public UpdatesDataLayerAppender(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task OnUpdateAsync(Update update, string source)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await db.Database.EnsureCreatedAsync();
            
            await db.Updates.AddAsync(update);

            await db.SaveChangesAsync();
        }
    }
}