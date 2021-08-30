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
        public static readonly string[] Platforms =
        {
            "facebook",
            "twitter",
            "feeds"
        };

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
            InlineKeyboardButton ToButton(string platform)
            {
                return InlineKeyboardButton.WithCallbackData(
                    Dictionary.GetPlatform(platform),
                    $"{Route.Subscriptions}-{platform}");
            }
            
            IEnumerable<IEnumerable<InlineKeyboardButton>> platformButtons = Platforms
                .Select(ToButton)
                .Concat(new []
                {
                    InlineKeyboardButton.WithCallbackData(
                        Dictionary.Back,
                        Route.Settings.ToString()), 
                })
                .Batch(1);
            
            return new InlineKeyboardMarkup(platformButtons);
        }
    }
}