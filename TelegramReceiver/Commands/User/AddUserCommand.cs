using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Telegram.Bot.Types.ReplyMarkups;
using SubscriptionsDb;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    internal class AddUserCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly UserValidator _validator;
        private readonly TimeSpan _defaultInterval;
        private readonly ILogger<AddUserCommand> _logger;

        public AddUserCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository,
            ISubscriptionsManager subscriptionsManager,
            UserValidator validator,
            TelegramConfig config,
            ILogger<AddUserCommand> logger) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
            _subscriptionsManager = subscriptionsManager;
            _validator = validator;
            _defaultInterval = config.DefaultInterval;
            _logger = logger;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            var platform = SelectedPlatform ?? throw new NullReferenceException();
            
            await SendRequestMessage(platform, token);

            // Wait for the user to reply with desired answer
            Update nextUpdate = await GetNextMessage();

            if (nextUpdate == null)
            {
                return new NoRedirectResult();
            }
            
            Message message = nextUpdate.Message;

            string requestId = message.Text;
            try
            {
                string userId = await _validator.ValidateAsync(requestId, platform);

                await AddUser(message, userId, platform, token);

                return new RedirectResult(
                    Route.User,
                    Context with { Trigger = null, Subscription = new AsyncLazy<SubscriptionEntity>(() => _chatSubscriptionsRepository.GetAsync(userId, platform)) });
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to validate [{}] {}", platform, requestId);
                
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: Dictionary.UserNotFound,
                    replyToMessageId: message.MessageId,
                    cancellationToken: token);
                
                return new RedirectResult(
                    Route.Subscriptions,
                    Context with { Trigger = null, SelectedPlatform = SelectedPlatform });
            }
        }

        private Task SendRequestMessage(
            string platform,
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
            string userId,
            string platform,
            CancellationToken token)
        {   
            TimeSpan interval = _defaultInterval;

            var subscription = new Subscription(userId, platform, interval, DateTime.Now);

            if (! await _chatSubscriptionsRepository.ExistsAsync(userId, platform))
            {
                await _subscriptionsManager.Subscribe(subscription);
            }
            
            var chatSubscription = new UserChatSubscription
            {
                ChatInfo = new ChatInfo
                {
                    Id = ConnectedChat
                },
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

            await _chatSubscriptionsRepository.AddOrUpdateAsync(userId, platform, chatSubscription);

            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.Added} {userId}",
                replyToMessageId: message.MessageId,
                cancellationToken: token);
        }
    }
}