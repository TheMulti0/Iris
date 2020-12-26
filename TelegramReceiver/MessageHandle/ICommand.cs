using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramReceiver
{
    public interface ICommand
    {
        ITrigger[] Triggers { get; }

        Task OperateAsync(ITelegramBotClient client, Update update);
    }
}