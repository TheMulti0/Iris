using System;
using System.Threading.Tasks;
using HtmlCssToImage.Net;

namespace MessagesManager
{
    internal class TwitterScreenshotter : IWebsiteScreenshotter
    {
        private const string TweetHtml = "<blockquote class=\"twitter-tweet\" style=\"width: 300px;\" data-dnt=\"true\">\r\n<p lang=\"en\" dir=\"ltr\"></p>\r\n\r\n<a href=\"{TWEET_URL}\"></a>\r\n\r\n</blockquote> <script async src=\"https://platform.twitter.com/widgets.js\" charset=\"utf-8\"></script>";
        private const double DeviceScale = 3;
        private const string CssSelector = ".twitter-tweet";
        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(1500);
        
        private readonly IHtmlCssToImageClient _client;

        public TwitterScreenshotter(IHtmlCssToImageClient client)
        {
            _client = client;
        }

        public async Task<string> ScreenshotAsync(string url)
        {
            var html = TweetHtml.Replace("{TWEET_URL}", url);

            var request = new CreateImageRequest(html)
            {
                DeviceScale = DeviceScale,
                CssSelector = CssSelector,
                Delay = Delay
            };
            
            CreateImageResponse image = await _client.CreateImageAsync(request);

            return image.Url;
        }
    }
}