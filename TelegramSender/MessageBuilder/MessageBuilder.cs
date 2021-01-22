using Common;
using Update = Common.Update;

namespace TelegramSender
{
    public class MessageBuilder
    {
        private readonly Languages _languages;

        public MessageBuilder(Languages languages)
        {
            _languages = languages;
        }

        public MessageInfo Build(Update update, UserChatSubscription chatSubscription)
        {
            var languageDictionary = _languages.Dictionary[chatSubscription.Language];

            string typePrefix = GetTypePrefix(update, languageDictionary);

            string prefix =
                $"<a href=\"{update.Url}\">{chatSubscription.DisplayName}{typePrefix} ({languageDictionary.GetPlatform(update.Author.Platform)}):</a>\n\n\n";
            
            string suffix = $"\n\n\n{update.Url}";

            if (chatSubscription.SendScreenshotOnly && update.Screenshot != null)
            {
                string prefixx = chatSubscription.ShowPrefix
                    ? prefix
                    : string.Empty;

                string msg = prefixx + (chatSubscription.ShowSuffix ? suffix : string.Empty);
                
                return new MessageInfo(
                    msg,
                    new []{ new BytesPhoto(update.Screenshot) },
                    chatSubscription.ChatId);
            }

            string message = GetMessage(update, chatSubscription, prefix, suffix);

            return new MessageInfo(
                message,
                update.Media,
                chatSubscription.ChatId,
                DisableWebPagePreview: !update.IsLive);
        }

        private static string GetTypePrefix(Update update, LanguageDictionary languageDictionary)
        {
            if (update.Repost)
            {
                return $" {languageDictionary.Repost}";
            }
            if (update.IsLive)
            {
                return $" {languageDictionary.Live}";
            }
            return string.Empty;
        }

        private static string GetMessage(Update update, UserChatSubscription chatSubscription, string prefix, string suffix)
        {
            string message;
            
            if (chatSubscription.ShowPrefix)
            {
                message = prefix + update.Content;
            }
            else
            {
                message = update.Content;
            }

            if (chatSubscription.ShowSuffix)
            {
                message += suffix;
            }

            return message;
        }
    }
}