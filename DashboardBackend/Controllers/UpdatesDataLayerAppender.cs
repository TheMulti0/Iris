using System.Threading.Tasks;
using Common;
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

            await repository.AddAsync(new UpdateEntity
            {
                AuthorId = update.AuthorId,
                Content = update.Content,
                CreationDate = update.CreationDate,
                Media = update.Media,
                Repost = update.Repost,
                Url = update.Url
            });
        }
    }
}