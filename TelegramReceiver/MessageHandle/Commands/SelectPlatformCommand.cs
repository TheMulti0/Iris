using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class SelectPlatformCommand : ICommand
    {
        private readonly InlineKeyboardMarkup _platformsMarkup;

        public const string CallbackPath = "selectPlatform";
        public ITrigger[] Triggers { get; } = {
            new CallbackTrigger(CallbackPath)
        };

        public SelectPlatformCommand()
        {
            var platforms = new[]
            {
                "Facebook"
            };
            
            IEnumerable<InlineKeyboardButton> platformButtons = platforms
                .Select(platform => InlineKeyboardButton
                            .WithCallbackData(platform, $"{AddUserCommand.CallbackPath}-{platform}"));

            _platformsMarkup = new InlineKeyboardMarkup(platformButtons);
        }

        public Task OperateAsync(ITelegramBotClient client, Update update)
        {
            Message message = update.CallbackQuery.Message;

            return client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: "Select a platform",
                replyMarkup: _platformsMarkup);
        }
    }
}