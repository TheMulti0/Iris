using System.Drawing.Imaging;
using System.Threading.Tasks;
using Common;

namespace MessagesManager
{
    public class Screenshotter
    {
        private TwitterScreenshotter _twitter;
        
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