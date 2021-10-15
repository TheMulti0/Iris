using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using SubscriptionsDb;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramReceiver
{
    internal class SetTextCommand : BaseCommand, ICommand
    {
        public SetTextCommand(Context context) : base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            TextType type = GetTextType();
            SubscriptionEntity entity = await Subscription;
            
            UserChatSubscription subscription = entity.Chats.First(info => info.ChatInfo.Id == ConnectedChat);

            Text text = type == TextType.Prefix 
                ? subscription.Prefix 
                : subscription.Suffix;

            var typeString = type == TextType.Prefix ? Dictionary.Prefix : Dictionary.Suffix;
            
            await Client.EditMessageTextAsync(
                chatId: ContextChat,
                messageId: Trigger.GetMessageId(),
                text: $"{Dictionary.SettingsFor} {typeString}",
                replyMarkup: GetMarkup(entity, text),
                cancellationToken: token);
            
            return new NoRedirectResult();
        }

        private InlineKeyboardMarkup GetMarkup(
            SubscriptionEntity entity,
            Text text)
        {
            var setContentButton = new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.SetTextContent,
                    $"{Route.SetTextContent}-{GetTextType()}-{entity.Id}")
            };
            
            var enabledButtons = new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Disable,
                    $"{Route.ToggleText}-{GetTextType()}-{entity.Id}"),

                InlineKeyboardButton.WithCallbackData(
                    $"{Dictionary.Mode}: {Dictionary.GetTextMode(text.Mode)}",
                    $"{Route.ToggleTextMode}-{GetTextType()}-{entity.Id}"),
                
                InlineKeyboardButton.WithCallbackData(
                    $"{Dictionary.Style}: {Dictionary.GetTextStyle(text.Style)}",
                    $"{Route.ToggleTextStyle}-{GetTextType()}-{entity.Id}")
            };

            var disabledButtons = new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Enable,
                    $"{Route.ToggleText}-{GetTextType()}-{entity.Id}")
            };
            
            IEnumerable<InlineKeyboardButton> backButton = new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Back,
                    $"{Route.User}-{entity.Id}")
            };

            if (text.Mode is TextMode.HyperlinkedText or TextMode.Text)
            {
                enabledButtons = enabledButtons.Concat(setContentButton).ToArray();
            }

            if (text.Enabled)
            {
                return new InlineKeyboardMarkup(
                    enabledButtons.Concat(backButton).Batch(1));
            }
            
            return new InlineKeyboardMarkup(
                disabledButtons.Concat(backButton).Batch(1));
        }
    }
}