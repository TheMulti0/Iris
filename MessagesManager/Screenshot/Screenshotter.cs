using System.Drawing.Imaging;
using System.Threading.Tasks;
using Common;

namespace MessagesManager
{
    internal class Screenshotter
    {
        private readonly TwitterScreenshotter _twitter;

        public Screenshotter(TwitterScreenshotter twitter)
        {
            _twitter = twitter;
        }

        public async Task<byte[]> ScreenshotAsync(Update update)
        {
            switch (update.Author.Platform)
            {
                case Platform.Twitter:
                    return _twitter.Screenshot(update.Url).ToByteArray(ImageFormat.Png);
                    
                default:
                    return null;
            }
        }
    }
}