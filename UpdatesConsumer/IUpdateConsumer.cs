using System.Threading.Tasks;
using Common;

namespace UpdatesConsumer
{
    public interface IUpdateConsumer
    {
        public Task OnUpdateAsync(Update update);
    }
}