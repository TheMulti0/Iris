using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Nito.AsyncEx;
using Telegram.Bot.Types.ReplyMarkups;
using SubscriptionsDb;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class AddUserCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;
        private readonly IProducer<ChatSubscriptionRequest> _producer;
        private readonly UserValidator _validator;
        private readonly TimeSpan _defaultInterval;

        public AddUserCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository,
            IProducer<ChatSubscriptionRequest> producer,
            UserValidator validator,
            TelegramConfig config) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
            _producer = producer;
            _validator = validator;
            _defaultInterval = config.DefaultInterval;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            Platform platform = SelectedPlatform ?? throw new NullReferenceException();
            
            await SendRequestMessage(platform, token);

            // Wait for the user to reply with desired answer
            Update nextUpdate = await GetNextMessage();

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
                Context with { Trigger = null, Subscription = new AsyncLazy<SubscriptionEntity>(() => _chatSubscriptionsRepository.GetAsync(user)) });
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

            var subscription = new Subscription(user, interval, DateTime.Now);

            if (! await _chatSubscriptionsRepository.ExistsAsync(user))
            {
                _producer.Send(
                    new ChatSubscriptionRequest(
                        SubscriptionType.Subscribe,
                        subscription,
                        ConnectedChat));
            }
            
            var chatSubscription = new UserChatSubscription
            {
                ChatId = ConnectedChat,
                Interval = interval,
                Prefix = new Text
                {
                    Enabled = false,
                    Content = string.Empty,
                    Mode = TextMode.Text
                },
                Suffix = new Text
                {
                    Enabled = false,
                    Content = string.Empty,
                    Mode = TextMode.Text
                },
                SubscriptionDate = DateTime.Now
            };

            await _chatSubscriptionsRepository.AddOrUpdateAsync(user, chatSubscription);

            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.Added} {user.UserId}",
                replyToMessageId: message.MessageId,
                cancellationToken: token);
        }
    }
}