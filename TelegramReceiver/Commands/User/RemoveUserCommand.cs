using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class RemoveUserCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IProducer<ChatSubscriptionRequest> _producer;

        public RemoveUserCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatSubscriptionRequest> producer): base(context)
        {
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
        }
        
        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup = CreateMarkup();

            await Remove(inlineKeyboardMarkup, token);

            return new RedirectResult(Route.Subscriptions);
        }

        private async Task Remove(
            InlineKeyboardMarkup markup,
            CancellationToken token)
        {
            var userPollRule = new Subscription(SelectedUser, null);
            
            _producer.Send(
                new ChatSubscriptionRequest(
                    SubscriptionType.Unsubscribe,
                    userPollRule,
                    ConnectedChat));
            
            await _savedUsersRepository.RemoveAsync(SelectedUser, ConnectedChat);

            await Client.EditMessageTextAsync(
                messageId: Trigger.GetMessageId(),
                chatId: ContextChat,
                text: $"{Dictionary.Removed} ({SelectedUser.UserId})",
                replyMarkup: markup,
                cancellationToken: token);
        }

        private InlineKeyboardMarkup CreateMarkup()
        {
            (string userId, Platform platform) = SelectedUser;
            
            return new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Back,
                    $"{Route.User}-{userId}-{Enum.GetName(platform)}"));
        }
    }
}