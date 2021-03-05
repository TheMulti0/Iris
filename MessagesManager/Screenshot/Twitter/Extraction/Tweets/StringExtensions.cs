using Extensions;

namespace MessagesManager
{
    public static class StringExtensions
    {
        internal static string CleanText(this string text)
        {
            return text.Replace(
                new[]
                {
                    @"(https://)",
                    "\r",
                    "\n"
                },
                string.Empty);
        }
    }
}