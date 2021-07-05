using System;
using System.Threading.Tasks;
using HtmlCssToImage.Net;

namespace MessagesManager
{
    public class TweetScreenshotter
    {
        private const string TweetHtml = "<blockquote class=\"twitter-tweet\" style=\"width: 400px;\" data-dnt=\"true\">\r\n<p lang=\"en\" dir=\"ltr\"></p>\r\n\r\n<a href=\"{TWEET_URL}\"></a>\r\n\r\n</blockquote> <script async src=\"https://platform.twitter.com/widgets.js\" charset=\"utf-8\"></script>";
        private const double DeviceScale = 2.5;
        private const string CssSelector = ".twitter-tweet";
        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(1500);
        
        private readonly IHtmlCssToImageClient _client;

        public TweetScreenshotter(HtmlToCssImageConfig config)
        {
            _client = new HtmlCssToImageClient(new HtmlCssToImageCredentials(config.UserId, config.ApiKey));
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