using System.Collections.Generic;
using Telegram.Bot.Types;
using Message = Common.Message;
using Update = Common.Update;

namespace TelegramSender
{
    public class MessageBuilder
    {
        public MessageInfo Build(Message message, ChatId chatId)
        {
            (Update update, List<string> _) = message;
            
            string repostPrefix = update.Repost ? " בפרסום מחדש" : string.Empty;

            // TODO show author display name
            var messageContent = $"<a href=\"{update.Url}\">{update.Author.DisplayName}{repostPrefix} ({update.Source}):</a>\n\n\n{update.Content}";

            return new MessageInfo(
                messageContent,
                update.Media,
                chatId);
        }
    }
}