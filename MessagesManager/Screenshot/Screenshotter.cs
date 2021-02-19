using System;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;

namespace MessagesManager
{
    internal class Screenshotter
    {
        private readonly TwitterScreenshotter _twitter;
        private readonly ILogger<Screenshotter> _logger;

        public Screenshotter(
            TwitterScreenshotter twitter,
            ILogger<Screenshotter> logger)
        {
            _twitter = twitter;
            _logger = logger;
        }

        public async Task<byte[]> ScreenshotAsync(Update update)
        {
            try
            {
                return update.Author.Platform switch 
                {
                    Platform.Twitter 
                        => _twitter.Screenshot(update.Url).ToByteArray(ImageFormat.Png),
                    _ 
                        => null
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to screenshot {}", update);
                return null;
            }
        }
    }
}