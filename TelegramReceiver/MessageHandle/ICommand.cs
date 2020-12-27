using System.Threading.Tasks;

namespace TelegramReceiver
{
    public interface ICommand
    {
        ITrigger[] Triggers { get; }

        Task OperateAsync(Context context);
    }
}