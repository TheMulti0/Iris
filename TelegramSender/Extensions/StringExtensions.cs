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
    }
}