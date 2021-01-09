using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramReceiver
{
    internal class SelectPlatformCommand : BaseCommand, ICommand
    {
        public SelectPlatformCommand(Context context): base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            await Client.EditMessageTextAsync(
                chatId: ContextChat,
                messageId: Trigger.GetMessageId(),
                text: Dictionary.SelectPlatform,
                replyMarkup: GetMarkup(),
                cancellationToken: token);

            return new NoRedirectResult();
        }
        
        private InlineKeyboardMarkup GetMarkup()
        {
            InlineKeyboardButton ToButton(Platform platform)
            {
                return InlineKeyboardButton.WithCallbackData(
                    Dictionary.GetPlatform(platform),
                    $"{Route.AddUser.ToString()}-{Enum.GetName(platform)}");
            }
            
            IEnumerable<IEnumerable<InlineKeyboardButton>> userButtons = Enum.GetValues<Platform>()
                .Select(ToButton)
                .Batch(2)
                .Concat(
                    new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                Dictionary.Back,
                                Route.Users.ToString())                            
                        }
                    });
            
            return new InlineKeyboardMarkup(userButtons);
        }
    }
}