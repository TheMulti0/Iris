using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Audio = UpdatesConsumer.Audio;

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

            var inputOnlineFile = new InputMedia(
                await _httpClient.GetStreamAsync(audio.Url, message.CancellationToken),
                "Audio");

            await _client.SendAudioAsync(
                chatId: message.ChatId,
                audio: inputOnlineFile,
                duration: audio.DurationSeconds ?? default,
                performer: audio.Artist,
                title: audio.Title,
                parseMode: TelegramConstants.MessageParseMode,
                replyToMessageId: message.ReplyMessageId,
                cancellationToken: message.CancellationToken
            );
        }
    }
}