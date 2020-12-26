using Telegram.Bot.Types;

namespace TelegramReceiver
{
    internal class AnyMessageTrigger : ITrigger
    {
        public bool ShouldTrigger(Update update)
        {
            return update.Message != null;
        }
    }
}