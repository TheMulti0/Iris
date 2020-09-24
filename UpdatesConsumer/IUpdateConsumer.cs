using System.Threading.Tasks;

namespace UpdatesConsumer
{
    public interface IUpdateConsumer
    {
        public Task OnUpdateAsync(Update update, string source);
    }
}