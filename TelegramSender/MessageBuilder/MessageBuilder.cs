using Common;
using Update = Common.Update;

namespace TelegramSender
{
    public class MessageBuilder
    {
        public MessageInfo Build(Update update, UserChatInfo chatInfo)
        {
            string repostPrefix = update.Repost ? " בפרסום מחדש" : string.Empty;

            // TODO show author display name
            var messageContent = $"<a href=\"{update.Url}\">{chatInfo.DisplayName}{repostPrefix} ({update.Source}):</a>\n\n\n{update.Content}";

            return new MessageInfo(
                messageContent,
                update.Media,
                chatInfo.ChatId);
        }
    }
}