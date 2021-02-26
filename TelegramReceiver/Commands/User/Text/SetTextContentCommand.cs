using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using SubscriptionsDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    internal class SetTextContentCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _repository;
        
        public SetTextContentCommand(
            Context context,
            IChatSubscriptionsRepository repository) : base(context)
        {
            _repository = repository;
        }
        
        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            CallbackQuery query = Trigger.CallbackQuery;

            InlineKeyboardMarkup inlineKeyboardMarkup = CreateMarkup(await Subscription);

            await SendRequestMessage(query.Message, inlineKeyboardMarkup, token);

            // Wait for the user to reply with desired text content

            var update = await GetNextMessage();

            if (update == null)
            {
                return new NoRedirectResult();
            }

            await SetTextContent(await Subscription, update);

            return new RedirectResult(Route.User, Context with { Trigger = null });
        }

        private InlineKeyboardMarkup CreateMarkup(SubscriptionEntity user)
        {
            return new(
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Back,
                    $"{Route.SetText}-{GetTextType()}-{user.Id}"));
        }

        private Task SendRequestMessage(
            Message message,
            InlineKeyboardMarkup markup,
            CancellationToken token)
        {
            return Client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: Dictionary.EnterContent,
                replyMarkup: markup,
                cancellationToken: token);
        }

        private async Task SetTextContent(
            SubscriptionEntity entity,
            Update update)
        {
            UserChatSubscription chat = entity.Chats.First(info => info.ChatId == ConnectedChat);

            string newContent = update.Message.Text;

            if (GetTextType() == TextType.Prefix)
            {
                chat.Prefix.Content = newContent;
            }
            else
            {
                chat.Suffix.Content = newContent;
            }
            
            await _repository.AddOrUpdateAsync(entity.User, chat);
        }
    }
}