using System;

namespace TelegramSender
{
    public static class StringExtensions
    {
        public static int LastIndexOfAny(this string str, string[] anyOf)
        {
            foreach (string s in anyOf)
            {
                int lastIndex = str.LastIndexOf(s, StringComparison.Ordinal);
                if (lastIndex != -1)
                {
                    return lastIndex + s.Length - 1;
                }
            }
            
            return -1;
        }
        
        // From https://stackoverflow.com/a/8809437
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text[..pos] + replace + text[(pos + search.Length)..];
        }
    }
}