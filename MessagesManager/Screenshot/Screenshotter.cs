using System.Threading.Tasks;
using Common;

namespace MessagesManager
{
    public class Screenshotter
    {
        public async Task<byte[]> ScreenshotAsync(Update update)
        {
            switch (update.Author.Platform)
            {
                case Platform.Twitter:
                    return await Tweetshot.ScreenshotAsync(update.Url);
                    
                default:
                    return null;
            }
        }
    }
}