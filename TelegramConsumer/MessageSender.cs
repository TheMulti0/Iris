using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace TelegramConsumer
{
    public class MessageSender
    {
        private readonly ILogger<MessageSender> _logger;
        private readonly TextSender _textSender;
        private readonly AudioSender _audioSender;
        private readonly MediaSender _mediaSender;

        public MessageSender(
            ITelegramBotClient client,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MessageSender>();

            _textSender = new TextSender(
                client,
                loggerFactory.CreateLogger<TextSender>());

            _audioSender = new AudioSender(
                client,
                loggerFactory.CreateLogger<AudioSender>());
            
            _mediaSender = new MediaSender(
                client,
                _textSender,
                loggerFactory.CreateLogger<MediaSender>());
        }

        public Task SendAsync(MessageInfo message)
        {
            _logger.LogWarning(
                "Message length: {}",
                message.Message.Length);

            if (message.Media.Any(media => media is Audio))
            {
                return _audioSender.SendAsync(
                    message,
                    (Audio) message.Media.FirstOrDefault(media => media is Audio));
            }

            return message.Media.Length switch 
            {
                0 => _textSender.SendAsync(message),
                _ => _mediaSender.SendAsync(message)
            };
        }
    }
}