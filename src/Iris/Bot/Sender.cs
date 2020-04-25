using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Update = Updates.Api.Update;

namespace Iris.Bot
{
    internal class Sender
    {
        private ITelegramBotClient _client;
        private ILogger<Sender> _logger;

        public Sender(
            ITelegramBotClient client,
            ILogger<Sender> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task SendAsync(Update update, long chatId)
        {
            Message[] previousMessages = null;
            if (update.Media.Any())
            {
                IEnumerable<IAlbumInputMedia> telegramMedia = update.Media
                    .Select(TelegramMediaFactory.ToTelegramMedia);
                    
                previousMessages = await _client.SendMediaGroupAsync(telegramMedia, chatId);
            }

            await _client.SendTextMessageAsync(
                chatId,
                update.FormattedMessage,
                ParseMode.Html,
                replyToMessageId: previousMessages?.LastOrDefault()?.MessageId ?? 0);
        }
    }
}