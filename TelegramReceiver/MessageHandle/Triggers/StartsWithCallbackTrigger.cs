using Telegram.Bot.Types;

namespace TelegramReceiver
{
    internal class StartsWithCallbackTrigger : ITrigger
    {
        private readonly string _callback;

        public StartsWithCallbackTrigger(string callback)
        {
            _callback = callback;
        }

        public bool ShouldTrigger(Update update)
        {
            return update.CallbackQuery?.Data.StartsWith(_callback) == true;
        }
    }
}