using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace TelegramConsumer.Tests
{
    internal class MockTelegramBotClient : ITelegramBotClient
    {
        private readonly ILogger<MockTelegramBotClient> _logger;

        public MockTelegramBotClient(ILogger<MockTelegramBotClient> logger)
        {
            _logger = logger;
        }

        public int BotId { get; }
        public TimeSpan Timeout { get; set; }
        public bool IsReceiving { get; }
        public int MessageOffset { get; set; }

        public event EventHandler<ApiRequestEventArgs> MakingApiRequest;
        public event EventHandler<ApiResponseEventArgs> ApiResponseReceived;
        public event EventHandler<UpdateEventArgs> OnUpdate;
        public event EventHandler<MessageEventArgs> OnMessage;
        public event EventHandler<MessageEventArgs> OnMessageEdited;
        public event EventHandler<InlineQueryEventArgs> OnInlineQuery;
        public event EventHandler<ChosenInlineResultEventArgs> OnInlineResultChosen;
        public event EventHandler<CallbackQueryEventArgs> OnCallbackQuery;
        public event EventHandler<ReceiveErrorEventArgs> OnReceiveError;
        public event EventHandler<ReceiveGeneralErrorEventArgs> OnReceiveGeneralError;

        public Task<TResponse> MakeRequestAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<bool> TestApiAsync(CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public void StartReceiving(
            UpdateType[] allowedUpdates = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public void StopReceiving() => throw new NotImplementedException();

        public Task<Telegram.Bot.Types.Update[]> GetUpdatesAsync(
            int offset = 0,
            int limit = 0,
            int timeout = 0,
            IEnumerable<UpdateType> allowedUpdates = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task SetWebhookAsync(
            string url,
            InputFileStream certificate = null,
            int maxConnections = 0,
            IEnumerable<UpdateType> allowedUpdates = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task DeleteWebhookAsync(CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public Task<WebhookInfo> GetWebhookInfoAsync(CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public Task<Telegram.Bot.Types.User> GetMeAsync(CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public Task<Message> SendTextMessageAsync(
            ChatId chatId,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _logger.LogInformation("Sending text message: {}", text);
            
            return Task.FromResult(new Message());
        }

        public Task<Message> ForwardMessageAsync(
            ChatId chatId,
            ChatId fromChatId,
            int messageId,
            bool disableNotification = false,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> SendPhotoAsync(
            ChatId chatId,
            InputOnlineFile photo,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _logger.LogInformation("Sending photo with url: {} and caption: {}", photo.Url, caption);
            
            return Task.FromResult(new Message());
        }

        public Task<Message> SendAudioAsync(
            ChatId chatId,
            InputOnlineFile audio,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            int duration = 0,
            string performer = null,
            string title = null,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            InputMedia thumb = null)
        {
            _logger.LogInformation("Sending audio with url: {} and caption: {}", audio.Url, caption);
            
            return Task.FromResult(new Message());
        }

        public Task<Message> SendDocumentAsync(
            ChatId chatId,
            InputOnlineFile document,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            InputMedia thumb = null) => throw new NotImplementedException();

        public Task<Message> SendStickerAsync(
            ChatId chatId,
            InputOnlineFile sticker,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> SendVideoAsync(
            ChatId chatId,
            InputOnlineFile video,
            int duration = 0,
            int width = 0,
            int height = 0,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            bool supportsStreaming = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            InputMedia thumb = null)
        {
            _logger.LogInformation("Sending video with url: {} and caption: {}", video.Url, caption);
            
            return Task.FromResult(new Message());
        }

        public Task<Message> SendAnimationAsync(
            ChatId chatId,
            InputOnlineFile animation,
            int duration = 0,
            int width = 0,
            int height = 0,
            InputMedia thumb = null,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> SendVoiceAsync(
            ChatId chatId,
            InputOnlineFile voice,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            int duration = 0,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> SendVideoNoteAsync(
            ChatId chatId,
            InputTelegramFile videoNote,
            int duration = 0,
            int length = 0,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            InputMedia thumb = null) => throw new NotImplementedException();

        public Task<Message[]> SendMediaGroupAsync(
            ChatId chatId,
            IEnumerable<InputMediaBase> media,
            bool disableNotification = false,
            int replyToMessageId = 0,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message[]> SendMediaGroupAsync(
            IEnumerable<IAlbumInputMedia> inputMedia,
            ChatId chatId,
            bool disableNotification = false,
            int replyToMessageId = 0,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _logger.LogInformation("Sending media group");
            
            return Task.FromResult(new[] { new Message() });
        }

        public Task<Message> SendLocationAsync(
            ChatId chatId,
            float latitude,
            float longitude,
            int livePeriod = 0,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> SendVenueAsync(
            ChatId chatId,
            float latitude,
            float longitude,
            string title,
            string address,
            string foursquareId = null,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            string foursquareType = null) => throw new NotImplementedException();

        public Task<Message> SendContactAsync(
            ChatId chatId,
            string phoneNumber,
            string firstName,
            string lastName = null,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            string vCard = null) => throw new NotImplementedException();

        public Task<Message> SendPollAsync(
            ChatId chatId,
            string question,
            IEnumerable<string> options,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            bool? isAnonymous = null,
            PollType? type = null,
            bool? allowsMultipleAnswers = null,
            int? correctOptionId = null,
            bool? isClosed = null,
            string explanation = null,
            ParseMode explanationParseMode = ParseMode.Default,
            int? openPeriod = null,
            DateTime? closeDate = null) => throw new NotImplementedException();

        public Task<Message> SendDiceAsync(
            ChatId chatId,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            Emoji? emoji = null) => throw new NotImplementedException();

        public Task SendChatActionAsync(
            ChatId chatId,
            ChatAction chatAction,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<UserProfilePhotos> GetUserProfilePhotosAsync(
            int userId,
            int offset = 0,
            int limit = 0,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<File> GetFileAsync(string fileId, CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public Task<Stream> DownloadFileAsync(
            string filePath,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task DownloadFileAsync(
            string filePath,
            Stream destination,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<File> GetInfoAndDownloadFileAsync(
            string fileId,
            Stream destination,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task KickChatMemberAsync(
            ChatId chatId,
            int userId,
            DateTime untilDate = new DateTime(),
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task LeaveChatAsync(ChatId chatId, CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public Task UnbanChatMemberAsync(
            ChatId chatId,
            int userId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Chat> GetChatAsync(ChatId chatId, CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public Task<ChatMember[]> GetChatAdministratorsAsync(
            ChatId chatId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<int> GetChatMembersCountAsync(
            ChatId chatId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<ChatMember> GetChatMemberAsync(
            ChatId chatId,
            int userId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task AnswerCallbackQueryAsync(
            string callbackQueryId,
            string text = null,
            bool showAlert = false,
            string url = null,
            int cacheTime = 0,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task RestrictChatMemberAsync(
            ChatId chatId,
            int userId,
            ChatPermissions permissions,
            DateTime untilDate = new DateTime(),
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task PromoteChatMemberAsync(
            ChatId chatId,
            int userId,
            bool? canChangeInfo = null,
            bool? canPostMessages = null,
            bool? canEditMessages = null,
            bool? canDeleteMessages = null,
            bool? canInviteUsers = null,
            bool? canRestrictMembers = null,
            bool? canPinMessages = null,
            bool? canPromoteMembers = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task SetChatAdministratorCustomTitleAsync(
            ChatId chatId,
            int userId,
            string customTitle,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task SetChatPermissionsAsync(
            ChatId chatId,
            ChatPermissions permissions,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<BotCommand[]> GetMyCommandsAsync(CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public Task SetMyCommandsAsync(
            IEnumerable<BotCommand> commands,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> EditMessageTextAsync(
            ChatId chatId,
            int messageId,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task EditMessageTextAsync(
            string inlineMessageId,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> StopMessageLiveLocationAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task StopMessageLiveLocationAsync(
            string inlineMessageId,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> EditMessageCaptionAsync(
            ChatId chatId,
            int messageId,
            string caption,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            ParseMode parseMode = ParseMode.Default) => throw new NotImplementedException();

        public Task EditMessageCaptionAsync(
            string inlineMessageId,
            string caption,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            ParseMode parseMode = ParseMode.Default) => throw new NotImplementedException();

        public Task<Message> EditMessageMediaAsync(
            ChatId chatId,
            int messageId,
            InputMediaBase media,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task EditMessageMediaAsync(
            string inlineMessageId,
            InputMediaBase media,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> EditMessageReplyMarkupAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task EditMessageReplyMarkupAsync(
            string inlineMessageId,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> EditMessageLiveLocationAsync(
            ChatId chatId,
            int messageId,
            float latitude,
            float longitude,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task EditMessageLiveLocationAsync(
            string inlineMessageId,
            float latitude,
            float longitude,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Poll> StopPollAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task DeleteMessageAsync(
            ChatId chatId,
            int messageId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task AnswerInlineQueryAsync(
            string inlineQueryId,
            IEnumerable<InlineQueryResultBase> results,
            int? cacheTime = null,
            bool isPersonal = false,
            string nextOffset = null,
            string switchPmText = null,
            string switchPmParameter = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> SendInvoiceAsync(
            int chatId,
            string title,
            string description,
            string payload,
            string providerToken,
            string startParameter,
            string currency,
            IEnumerable<LabeledPrice> prices,
            string providerData = null,
            string photoUrl = null,
            int photoSize = 0,
            int photoWidth = 0,
            int photoHeight = 0,
            bool needName = false,
            bool needPhoneNumber = false,
            bool needEmail = false,
            bool needShippingAddress = false,
            bool isFlexible = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken(),
            bool sendPhoneNumberToProvider = false,
            bool sendEmailToProvider = false) => throw new NotImplementedException();

        public Task AnswerShippingQueryAsync(
            string shippingQueryId,
            IEnumerable<ShippingOption> shippingOptions,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task AnswerShippingQueryAsync(
            string shippingQueryId,
            string errorMessage,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task AnswerPreCheckoutQueryAsync(
            string preCheckoutQueryId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task AnswerPreCheckoutQueryAsync(
            string preCheckoutQueryId,
            string errorMessage,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> SendGameAsync(
            long chatId,
            string gameShortName,
            bool disableNotification = false,
            int replyToMessageId = 0,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<Message> SetGameScoreAsync(
            int userId,
            int score,
            long chatId,
            int messageId,
            bool force = false,
            bool disableEditMessage = false,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task SetGameScoreAsync(
            int userId,
            int score,
            string inlineMessageId,
            bool force = false,
            bool disableEditMessage = false,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<GameHighScore[]> GetGameHighScoresAsync(
            int userId,
            long chatId,
            int messageId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<GameHighScore[]> GetGameHighScoresAsync(
            int userId,
            string inlineMessageId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<StickerSet> GetStickerSetAsync(
            string name,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<File> UploadStickerFileAsync(
            int userId,
            InputFileStream pngSticker,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task CreateNewStickerSetAsync(
            int userId,
            string name,
            string title,
            InputOnlineFile pngSticker,
            string emojis,
            bool isMasks = false,
            MaskPosition maskPosition = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task AddStickerToSetAsync(
            int userId,
            string name,
            InputOnlineFile pngSticker,
            string emojis,
            MaskPosition maskPosition = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task CreateNewAnimatedStickerSetAsync(
            int userId,
            string name,
            string title,
            InputFileStream tgsSticker,
            string emojis,
            bool isMasks = false,
            MaskPosition maskPosition = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task AddAnimatedStickerToSetAsync(
            int userId,
            string name,
            InputFileStream tgsSticker,
            string emojis,
            MaskPosition maskPosition = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task SetStickerPositionInSetAsync(
            string sticker,
            int position,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task DeleteStickerFromSetAsync(
            string sticker,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task SetStickerSetThumbAsync(
            string name,
            int userId,
            InputOnlineFile thumb = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task<string> ExportChatInviteLinkAsync(
            ChatId chatId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task SetChatPhotoAsync(
            ChatId chatId,
            InputFileStream photo,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task DeleteChatPhotoAsync(ChatId chatId, CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public Task SetChatTitleAsync(
            ChatId chatId,
            string title,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task SetChatDescriptionAsync(
            ChatId chatId,
            string description = null,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task PinChatMessageAsync(
            ChatId chatId,
            int messageId,
            bool disableNotification = false,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task UnpinChatMessageAsync(ChatId chatId, CancellationToken cancellationToken = new CancellationToken())
            => throw new NotImplementedException();

        public Task SetChatStickerSetAsync(
            ChatId chatId,
            string stickerSetName,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public Task DeleteChatStickerSetAsync(
            ChatId chatId,
            CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();
    }
}