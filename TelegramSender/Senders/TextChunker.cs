using System;
using System.Collections.Generic;
using System.Linq;

namespace TelegramSender
{
    internal class TextChunker
    {
        public static IEnumerable<string> ChunkText(string text)
        {
            int maxLength = TelegramConstants.MaxTextMessageLength;
            var suffix = "\n>>>";
            
            IEnumerable<string> segments = CutText(
                text,
                maxLength,
                suffix,
                "**",
                "--",
                "\".",
                "\n",
                "?",
                "!",
                ".",
                ":",
                ",").ToList();
            
            int maxChunkLength = TelegramConstants.MaxTextMessageLength - suffix.Length;

            segments = GetMergedSegments(segments, maxChunkLength);
            segments = WithSuffix(segments, suffix);
            
            return segments;
        }

        private static IEnumerable<string> GetMergedSegments(IEnumerable<string> segments, int maxChunkLength)
        {
            bool shouldMerge = true;
            while (shouldMerge)
            {
                var merged = TryMergeLastSegments(segments, maxChunkLength);

                shouldMerge = merged.Count() < segments.Count();

                if (shouldMerge)
                {
                    segments = merged;
                }
            }
            
            return segments;
        }

        private static IEnumerable<string> CutText(
            string str,
            int maxLength,
            string suffix,
            params string[] keywords)
        {
            var chunkIndex = 0;
            var charIndex = 0;

            int length = str.Length;
            int maxChunkLength = maxLength - suffix.Length;
            
            while (charIndex < length)
            {
                if (chunkIndex == length - 1)
                {
                    maxChunkLength = maxLength;
                }

                yield return Cut(str, maxChunkLength, keywords, charIndex, out int endIndex);

                chunkIndex++;
                charIndex += endIndex + 1;
            }
        }

        private static string Cut(
            string str,
            int maxLength,
            string[] keywords, 
            int charIndex,
            out int endCharIndex)
        {
            string chunk = charIndex + maxLength >= str.Length
                ? str.Substring(charIndex)
                : str.Substring(charIndex, maxLength);

            int endIndex = chunk.LastIndexOfAny(keywords);

            if (endIndex < 0)
                endIndex = chunk.LastIndexOf(" ", StringComparison.Ordinal);

            if (endIndex < 0)
                endIndex = Math.Min(maxLength - 1, chunk.Length - 1);

            endCharIndex = endIndex;
            
            return chunk.Substring(0, endIndex + 1);
        }

        private static IEnumerable<string> TryMergeLastSegments(IEnumerable<string> segments, int maxChunkLength)
        {
            string[] arr = segments.ToArray();
            
            foreach (var segment in arr.SkipLast(2))
            {
                yield return segment;
            }
            
            int length = arr[^2].Length + arr[^1].Length;

            if (length <= maxChunkLength)
            {
                yield return arr[^2] + arr[^1];
            }
            else
            {
                foreach (var segment in arr.Skip(arr.Length - 2))
                {
                    yield return segment;
                }
            }
        }

        private static IEnumerable<string> WithSuffix(IEnumerable<string> segments, string suffix)
        {
            var arr = segments.ToArray();
            
            foreach (string s in arr.SkipLast(1))
            {
                yield return s + suffix;
            }
            
            yield return arr.Last();
        }
    }
}