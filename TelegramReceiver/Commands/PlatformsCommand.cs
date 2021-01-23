using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramReceiver
{
    internal class PlatformsCommand : BaseCommand, ICommand
    {
        public PlatformsCommand(Context context): base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            if (Trigger.Type == UpdateType.CallbackQuery)
            {
                await Client.EditMessageTextAsync(
                    chatId: ContextChat,
                    messageId: Trigger.GetMessageId(),
                    text: Dictionary.SelectPlatform,
                    replyMarkup: GetMarkup(),
                    cancellationToken: token);
                
            }
            else
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: Dictionary.SelectPlatform,
                    replyMarkup: GetMarkup(),
                    cancellationToken: token);
            }

            return new NoRedirectResult();
        }
        
        private InlineKeyboardMarkup GetMarkup()
        {
            InlineKeyboardButton ToButton(Platform platform)
            {
                return InlineKeyboardButton.WithCallbackData(
                    Dictionary.GetPlatform(platform),
                    $"{Route.Subscriptions}-{Enum.GetName(platform)}");
            }
            
            IEnumerable<IEnumerable<InlineKeyboardButton>> platformButtons = Enum.GetValues<Platform>()
                .Except(new []
                {
                    Platform.Feeds
                })
                .Select(ToButton)
                .Batch(1);
            
            return new InlineKeyboardMarkup(platformButtons);
        }
    }
}