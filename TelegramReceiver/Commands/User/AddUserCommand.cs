using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class AddUserCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IProducer<ChatSubscriptionRequest> _producer;
        private readonly UserValidator _validator;
        private readonly TimeSpan _defaultInterval;

        public AddUserCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatSubscriptionRequest> producer,
            UserValidator validator,
            TelegramConfig config) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
            _validator = validator;
            _defaultInterval = config.DefaultInterval;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            Platform platform = SelectedPlatform ?? throw new NullReferenceException();
            
            await SendRequestMessage(platform, token);

            // Wait for the user to reply with desired answer
            Update nextUpdate = await NextMessage;

            if (nextUpdate == null)
            {
                return new NoRedirectResult();
            }
            
            Message message = nextUpdate.Message;
            
            var request = new User(message.Text, platform);
            var user = await _validator.ValidateAsync(request);

            if (user == null)
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: Dictionary.UserNotFound,
                    replyToMessageId: message.MessageId,
                    cancellationToken: token);
                
                return new RedirectResult(
                    Route.Subscriptions,
                    Context with { Trigger = null, SelectedPlatform = SelectedPlatform });
            }
            
            await AddUser(message, user, token);

            return new RedirectResult(
                Route.User,
                Context with { Trigger = null, SelectedUser = request });
        }

        private Task SendRequestMessage(
            Platform platform,
            CancellationToken token)
        {
            var inlineKeyboardMarkup = new InlineKeyboardMarkup(new []
            {
                InlineKeyboardButton
                    .WithCallbackData(
                        Dictionary.Back, 
                        $"{Route.Subscriptions}-{SelectedPlatform}"), 
            });
            
            return Client.EditMessageTextAsync(
                chatId: ContextChat,
                messageId: Trigger.GetMessageId(),
                text: $"{Dictionary.EnterUserFromPlatform} {Dictionary.GetPlatform(platform)}",
                replyMarkup: inlineKeyboardMarkup,
                cancellationToken: token);
        }
        
        private async Task AddUser(
            Message message,
            User user,
            CancellationToken token)
        {   
            TimeSpan interval = _defaultInterval;
            
            var subscription = new Subscription(user, interval);

            _producer.Send(
                new ChatSubscriptionRequest(
                    SubscriptionType.Subscribe,
                    subscription,
                    ConnectedChat));

            await _savedUsersRepository.AddOrUpdateAsync(
                user,
                new UserChatSubscription
                {
                    ChatId = ConnectedChat,
                    Interval = interval,
                    DisplayName = user.UserId,
                    Language = Language
                });

            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.Added} {user.UserId}",
                replyToMessageId: message.MessageId,
                cancellationToken: token);
        }
    }
}