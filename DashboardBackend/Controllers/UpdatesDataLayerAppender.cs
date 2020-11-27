using System.Threading.Tasks;
using DashboardBackend.Data;
using Microsoft.Extensions.DependencyInjection;
using UpdatesConsumer;

namespace DashboardBackend.Controllers
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

            var repository = scope.ServiceProvider.GetRequiredService<IUpdatesRepository>();

            await repository.AddAsync(update);
        }
    }
}