namespace Iris.Api
{
    public static class MessageFormatter
    {
        public static string FormatMessage(
            string postUrl,
            string authorName,
            string verb,
            string postText)
        {
            return $"<a href=\"{postUrl}\"> {authorName} {verb}: </a>\n \n \n{postText}\n \n";
        }
    }
}