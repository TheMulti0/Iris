using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;

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
            var platforms = Enum.GetNames<Platform>();
            
            IEnumerable<InlineKeyboardButton> platformButtons = platforms
                .Select(platform => InlineKeyboardButton
                            .WithCallbackData(platform, $"{AddUserCommand.CallbackPath}-{platform}"));

            _platformsMarkup = new InlineKeyboardMarkup(platformButtons);
        }

        public Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, _, Update update) = context;
            Message message = update.CallbackQuery.Message;

            return client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: "Select a platform",
                replyMarkup: _platformsMarkup);
        }
    }
}