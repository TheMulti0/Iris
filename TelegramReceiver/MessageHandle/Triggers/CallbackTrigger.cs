using Telegram.Bot.Types;

namespace TelegramReceiver
{
    internal class CallbackTrigger : ITrigger
    {
        private readonly string _callback;

        public CallbackTrigger(string callback)
        {
            _callback = callback;
        }

        public bool ShouldTrigger(Update update)
        {
            return update.CallbackQuery?.Data == _callback;
        }
    }
}