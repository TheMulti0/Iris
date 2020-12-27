using Telegram.Bot.Types;

namespace TelegramReceiver
{
    internal class MessageStartsWithTextTrigger : ITrigger
    {
        private readonly string _text;

        public MessageStartsWithTextTrigger(string text)
        {
            _text = text;
        }

        public bool ShouldTrigger(Update update)
        {
            return update.Message?.Text.StartsWith(_text) == true;
        }
    }
}