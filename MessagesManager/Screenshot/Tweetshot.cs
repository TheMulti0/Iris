using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace MessagesManager
{
    internal class Tweetshot
    {
        private const string TweetUrlPattern = @"^http(s)?:\/\/twitter\.com\/(?:#!\/)?(?<userId>\w+)\/status(es)?\/(?<tweetId>\d+)$";
        private static readonly Regex TweetUrlRegex = new(TweetUrlPattern);
        private static readonly SemaphoreSlim TweetshotLock = new(1, 1); 

        public static async Task<byte[]> ScreenshotAsync(
            string url,
            int scale = 2,
            bool darkMode = false,
            int quality = 100)
        {
            await TweetshotLock.WaitAsync();

            try
            {
                await ExecuteScreenshotAsync(url, scale, darkMode, quality);

                return await ReadScreenshotAsync(url, darkMode);
            }
            finally
            {
                TweetshotLock.Release();
            }
        }

        private static async Task ExecuteScreenshotAsync(string url, int scale, bool darkMode, int quality)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            
            try
            {
                await ScriptExecutor.Execute(
                    "node",
                    "tweet-shot.js",
                    token: cts.Token,
                    $"--url {url}",
                    $"--scale {scale}",
                    darkMode ? "--dark-mode" : string.Empty,
                    $"--quality {quality}");
            }
            catch (InvalidOperationException)
            {
            }
        }

        private static async Task<byte[]> ReadScreenshotAsync(string url, bool darkMode)
        {
            GroupCollection groups = TweetUrlRegex.Match(url).Groups;
            string userId = groups["userId"].Value;
            string tweetId = groups["tweetId"].Value;
            
            try
            {
                return await File.ReadAllBytesAsync($"{userId}-{tweetId}_{(darkMode ? "dark" : string.Empty)}.jpg");
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}