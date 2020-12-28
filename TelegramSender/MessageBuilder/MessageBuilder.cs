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

            var messageContent = $"<a href=\"{update.Url}\">{chatInfo.DisplayName}{repostPrefix} ({languageDictionary.GetPlatform(update.Author.Platform)}):</a>\n\n\n{update.Content}";

            return new MessageInfo(
                messageContent,
                update.Media,
                chatInfo.ChatId);
        }
    }
}