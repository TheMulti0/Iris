namespace MessagesManager
{
    public class TwitterScreenshotterConfig
    {
        public bool UseLocalChromeDriver { get; set; }
        
        public string RemoteChromeUrl { get; set; }

        public int BrowserWidth { get; set; } = 2400;

        public int BrowserHeight { get; set; } = 1080;
    }
}