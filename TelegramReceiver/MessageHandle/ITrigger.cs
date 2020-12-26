using Telegram.Bot.Types;

namespace TelegramReceiver
{
    public interface ITrigger
    {
        bool ShouldTrigger(Update update);
    }
}