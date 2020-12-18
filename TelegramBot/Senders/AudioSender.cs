using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Audio = Common.Audio;

namespace TelegramBot
{
    public class AudioSender
    {
        private readonly HttpClient _httpClient;
        private readonly ITelegramBotClient _client;
        private readonly ILogger<AudioSender> _logger;

        public AudioSender(
            ITelegramBotClient client,
            ILogger<AudioSender> logger)
        {
            _httpClient = new HttpClient();
            _client = client;
            _logger = logger;
        }

        public async Task SendAsync(MessageInfo message, Audio audio)
        {
            _logger.LogInformation("Sending audio message");

            InputMedia audioFile = audio.Url;

            await _client.SendAudioAsync(
                chatId: message.ChatId,
                audio: audioFile,
                caption: message.Message,
                thumb: audio.ThumbnailUrl,
                duration: audio.Duration?.Seconds ?? default,
                performer: audio.Artist,
                title: audio.Title,
                parseMode: TelegramConstants.MessageParseMode,
                replyToMessageId: message.ReplyMessageId,
                cancellationToken: message.CancellationToken
            );
        }
    }
}