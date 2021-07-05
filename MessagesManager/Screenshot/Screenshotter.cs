using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace MessagesManager
{
    internal class Screenshotter
    {
        private readonly Dictionary<Platform, IWebsiteScreenshotter> _screenshotters;

        public Screenshotter(
            TwitterScreenshotter twitter)
        {
            _screenshotters = new Dictionary<Platform, IWebsiteScreenshotter>
            {
                { Platform.Twitter, twitter }
            };
        }

        public async Task<Update> ScreenshotAsync(Update update)
        {
            Platform platform = update.Author.Platform;
            
            if (!_screenshotters.ContainsKey(platform))
            {
                throw new NotImplementedException($"{platform} has no supported screenshotter");
            }

            IWebsiteScreenshotter screenshotter = _screenshotters[platform];
            
            string screenshotUrl = await screenshotter.ScreenshotAsync(update.Url);

            var screenshot = new Photo(screenshotUrl);
            
            return update with { Content = string.Empty, Media = new List<IMedia> { screenshot } };
        }
    }
}