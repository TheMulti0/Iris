using System;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;

namespace MessagesManager
{
    internal class Screenshotter
    {
        private readonly IWebDriverFactory _webDriverFactory;
        private readonly ILogger<Screenshotter> _logger;

        public Screenshotter(
            IWebDriverFactory webDriverFactory,
            ILogger<Screenshotter> logger)
        {
            _webDriverFactory = webDriverFactory;
            _logger = logger;
        }

        public async Task<byte[]> ScreenshotAsync(Update update)
        {
            try
            {
                return update.Author.Platform switch 
                {
                    Platform.Twitter 
                        => new TwitterScreenshotter(_webDriverFactory.Create())
                            .Screenshot(update)
                            .ToByteArray(ImageFormat.Png),
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