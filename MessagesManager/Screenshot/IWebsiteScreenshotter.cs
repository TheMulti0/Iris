using System.Threading.Tasks;

namespace MessagesManager
{
    internal interface IWebsiteScreenshotter
    {
        Task<string> ScreenshotAsync(string url);
    }
}