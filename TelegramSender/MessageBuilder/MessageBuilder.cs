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

        public MessageInfo Build(Update update, UserChatInfo chatInfo)
        {
            var languageDictionary = _languages.Dictionary[chatInfo.Language];
            
            string repostPrefix = update.Repost 
                ? $" {languageDictionary.Repost}" 
                : string.Empty;

            string prefix =
                $"<a href=\"{update.Url}\">{chatInfo.DisplayName}{repostPrefix} ({languageDictionary.GetPlatform(update.Author.Platform)}):</a>\n\n\n";
            
            string suffix = $"\n\n\n{update.Url}";

            string message = GetMessage(update, chatInfo, prefix, suffix);

            return new MessageInfo(
                message,
                update.Media,
                chatInfo.ChatId);
        }

        private static string GetMessage(Update update, UserChatInfo chatInfo, string prefix, string suffix)
        {
            string message;
            
            if (chatInfo.ShowPrefix)
            {
                message = prefix + update.Content;
            }
            else
            {
                message = update.Content;
            }

            if (chatInfo.ShowSuffix)
            {
                message += suffix;
            }

            return message;
        }
    }
}