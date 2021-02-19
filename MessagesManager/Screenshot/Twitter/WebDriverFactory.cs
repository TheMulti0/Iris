using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace MessagesManager
{
    internal class WebDriverFactory : IWebDriverFactory
    {
        private readonly TwitterScreenshotterConfig _config;

        public WebDriverFactory(TwitterScreenshotterConfig config)
        {
            _config = config;
        }

        public IWebDriver Create()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            
            if (_config.UseLocalChromeDriver)
            {
                return new ChromeDriver(chromeOptions);
            }
            
            return new RemoteWebDriver(
                new Uri(_config.RemoteChromeUrl),
                chromeOptions);
        }
    }
}