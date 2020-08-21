namespace TelegramConsumer
{
    internal static class MessageBuilder
    {
        public static string Build(Update update, User user)
        {
            string repostPrefix = update.Repost ? " בפרסום מחדש" : string.Empty;

            return $"<a href=\"{update.Url}\">{user.DisplayName}{repostPrefix}:</a>\n \n \n{update.Content}";
        }
    }
}