namespace TelegramConsumer
{
    internal static class UpdateMessageFactory
    {
        public static UpdateMessage Create(Update update, User user)
        {
            string repostPrefix = update.Repost ? " בפרסום מחדש" : string.Empty;
            
            return new UpdateMessage
            {
                Message = $"<a href=\"{update.Url}\">{user.DisplayName}{repostPrefix}:</a>\n \n \n{update.Content}",
                Media = update.Media
            };
        }
    }
}