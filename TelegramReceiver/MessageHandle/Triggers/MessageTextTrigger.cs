using Telegram.Bot.Types;

namespace TelegramReceiver
{
    internal class MessageTextTrigger : ITrigger
    {
        private readonly string _text;

        public MessageTextTrigger(string text)
        {
            _text = text;
        }

        public bool ShouldTrigger(Update update)
        {
            return update.Message?.Text == _text;
        }
    }
}