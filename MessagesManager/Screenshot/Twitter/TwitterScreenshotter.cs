using System;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MessagesManager
{
    internal class TwitterScreenshotter
    {
        private const string TweetXPath = "/html/body/div/div/div/div[2]/main/div/div/div/div[1]/div/div[2]/div/section/div/div/div[1]/div/div/article";
        private static readonly string TweetImagesXPath = $"{TweetXPath}/div/div/div/div[3]/div[2]/div/div/div/div/div/div[2]/div";
        private static readonly string TweetDateXPath = $"{TweetXPath}/div/div/div/div[3]/div[3]";
        private static readonly string TweetStatsXPath = $"{TweetXPath}/div/div/div/div[3]/div[4]";
        private static readonly string TweetButtonsXPath = $"{TweetXPath}/div/div/div/div[3]/div[5]";

        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly WebDriverWait _shortWait;

        public TwitterScreenshotter(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));
        }

        public Bitmap Screenshot(string url)
        {
            Setup(url);

            var tweet = _wait.Until(GetElement(By.XPath(TweetXPath)));

            WaitForTweetThumbnails();

            Bitmap bitmap = Screenshot(tweet);
            
            _driver.Close();

            return bitmap;
        }

        private void Setup(string url)
        {
            _driver.Manage()
                .Window.Size = new Size(1080, 2400);

            _driver.Navigate()
                .GoToUrl(url);
        }

        private void WaitForTweetThumbnails()
        {
            try
            {
                _shortWait.Until(GetElement(By.XPath(TweetImagesXPath))); // Wait for tweet thumbnails
            }
            catch
            {
                // Tweet might be text so no images will be loaded
            }
        }

        private Bitmap Screenshot(IWebElement tweet)
        {
            var takesScreenshot = (ITakesScreenshot) tweet;
            
            return takesScreenshot
                .GetScreenshot()
                .AsByteArray
                .ToBitmap()
                .Crop(GetViewport(tweet.Size))
                .RoundCorners(30);
        }

        private Rectangle GetViewport(Size tweetSize)
        {
            var dateHeight = _driver.FindElement(By.XPath(TweetDateXPath)).Size.Height + 20;
            var statsHeight = _driver.FindElement(By.XPath(TweetStatsXPath)).Size.Height;
            var buttonsHeight = _driver.FindElement(By.XPath(TweetButtonsXPath)).Size.Height;

            var finalHeight = new Size(
                tweetSize.Width,
                tweetSize.Height - dateHeight - statsHeight - buttonsHeight);

            return new Rectangle(Point.Empty, finalHeight);
        }

        private static Func<IWebDriver, IWebElement> GetElement(By by) 
            => driver => driver.FindElement(by);
    }
}