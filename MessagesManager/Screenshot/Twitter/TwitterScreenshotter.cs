using System;
using System.Drawing;
using System.Linq;
using Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MessagesManager
{
    internal class TwitterScreenshotter
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private IExtractedTweet _extractedTweet;

        public TwitterScreenshotter(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public Bitmap Screenshot(Update update)
        {
            try
            {
                Setup(update.Url);

                _extractedTweet = ExtractTweet(update);

                Bitmap bitmap = Screenshot(_extractedTweet.GetTweet());

                _driver.Dispose();
                
                return bitmap;
            }
            catch
            {
                _driver.Dispose();
                throw;
            }
        }

        private IExtractedTweet ExtractTweet(Update update)
        {
            bool hasMedia = update.Media.Any();
            bool isReply = update.IsReply;
            
            if (isReply)
            {
                if (hasMedia)
                {
                    return new ExtractedReplyMediaTweet(_driver, update.Content, _wait);
                }
                
                return new ExtractedReplyTweet(_driver, update.Content, _wait);
            }
            
            if (hasMedia)
            {
                return new ExtractedMediaTweet(_driver, _wait);
            }
                
            return new ExtractedTweet(_driver, _wait);
        }

        private void Setup(string url)
        {
            _driver.Manage()
                .Window.Size = new Size(1080, 2400);

            _driver.Navigate()
                .GoToUrl(url);
        }

        private Bitmap Screenshot(IWebElement tweetElement)
        {
            var takesScreenshot = (ITakesScreenshot) tweetElement;

            TweetHeights tweetHeights = GetHeights();
            
            return takesScreenshot
                .GetScreenshot()
                .AsByteArray
                .ToBitmap()
                .Crop(GetViewport(tweetHeights, tweetElement.Size))
                .RoundCorners(20);
        }

        private TweetHeights GetHeights()
        {
            try
            {
                return _extractedTweet.GetHeights();
            }
            catch (NoSuchElementException)
            {
                return new TweetHeights(0, 0, 0);
            }
        }

        private static Rectangle GetViewport(TweetHeights tweetHeights, Size tweetSize)
        {
            (int dateHeight, int statsHeight, int buttonsHeight) = tweetHeights;

            var finalHeight = new Size(
                tweetSize.Width,
                tweetSize.Height 
                    - (dateHeight + 20) 
                    - statsHeight 
                    - buttonsHeight);

            return new Rectangle(Point.Empty, finalHeight);
        }
    }
}