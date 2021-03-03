using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MessagesManager
{
    public static class StringExtensions
    {
        internal static string CleanText(this string text)
        {
            return Replace(
                text,
                new[]
                {
                    @"(https://)",
                    "\r",
                    "\n"
                },
                string.Empty);
        }

        private static string Replace(
            string input,
            IEnumerable<string> patterns,
            string replacement)
        {
            string newestText = input;
            
            foreach (string pattern in patterns)
            {
                newestText = Regex.Replace(newestText, pattern, replacement);
            }

            return newestText;
        }

    }
}