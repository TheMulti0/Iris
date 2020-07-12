using System;

namespace TelegramConsumer
{
    internal static class UpdateMessageFactory
    {
        public static UpdateMessage Create(Update update, User user)
        {
            Console.WriteLine(user.DisplayName);
            return new UpdateMessage
            {
                Message = $"<a href=\"{update.Url}\"> {user.DisplayName}:</a>\n \n \n{update.Content}",
                Media = update.Media
            };
        }
    }
}