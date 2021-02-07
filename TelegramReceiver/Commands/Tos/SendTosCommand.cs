using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramReceiver
{
    internal class SendTosCommand : BaseCommand, ICommand
    {
        public SendTosCommand(
            Context context) : base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            string text = $"<b>{Dictionary.YouMustAgreeToTos}</b>\n \n {Dictionary.Tos}";
            var markup = new InlineKeyboardMarkup(GetActionButtons());

            if (Trigger.Type == UpdateType.CallbackQuery)
            {
                await Client.EditMessageTextAsync(
                    chatId: ContextChat,
                    messageId: Trigger.GetMessageId(),
                    text: text,
                    replyMarkup: markup,
                    parseMode: ParseMode.Html,
                    cancellationToken: token);
            }
            else
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: text,
                    replyMarkup: markup,
                    parseMode: ParseMode.Html,
                    cancellationToken: token);
            }

            return new NoRedirectResult();
        }

        private IEnumerable<IEnumerable<InlineKeyboardButton>> GetActionButtons()
        {
            return new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Accept,
                    Route.AcceptTos.ToString()),
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Decline,
                    Route.DeclineTos.ToString())
            }.Batch(1);
        }
    }
}