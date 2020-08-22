using System;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelegramConsumer.Tests
{
    [TestClass]
    public class MockTelegramBotTests
    {
        private static Result<TelegramConfig> _config;
        private static TelegramBot _bot;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(
                builder => builder.AddTestsLogging(context));

            _config = Result<TelegramConfig>.Failure("Default config");
            
            var configsProvider = new MockConfigProvider();
            configsProvider.Configs.Subscribe(result => _config = result);

            _bot = new TelegramBot(
                configsProvider,
                new MockTelegramBotClientProvider(loggerFactory),
                loggerFactory);
        }
        
        [TestMethod]
        public async Task TestText()
        {
            User user = GetFirstConfiguredUser();

            await _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Content = "Mock update"
                }, "test");
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestTextWithUrl()
        {
            User user = GetFirstConfiguredUser();

            await _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Content = "Mock update",
                    Url = "https://mock-url.com"
                }, "test");
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestPhoto()
        {
            User user = GetFirstConfiguredUser();

            await _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Media = new [] 
                    {   
                        new Media
                        {
                            Type = MediaType.Photo,
                            Url = "https://mock-photo-url.com"
                        } 
                    }
                }, "test");
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestPhotoWithDetails()
        {
            User user = GetFirstConfiguredUser();

            await _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Media = new [] 
                    {   
                        new Media
                        {
                            Type = MediaType.Photo,
                            Url = "https://mock-photo-url.com"
                        } 
                    },
                    Content = "Mock photo",
                    Url = "https://mock-url.com"
                }, "test");
            
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestVideo()
        {
            User user = GetFirstConfiguredUser();

            await _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Media = new [] 
                    {   
                        new Media
                        {
                            Type = MediaType.Video,
                            Url = "https://mock-video-url.com"
                        } 
                    }
                }, "test");
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestVideoWithDetails()
        {
            User user = GetFirstConfiguredUser();

            await _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Media = new [] 
                    {   
                        new Media
                        {
                            Type = MediaType.Video,
                            Url = "https://mock-video-url.com"
                        } 
                    },
                    Content = "Mock video",
                    Url = "https://mock-url.com"
                }, "test");
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestMultipleMediaWithDetails()
        {
            User user = GetFirstConfiguredUser();

            await _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Media = new [] 
                    {   
                        new Media
                        {
                            Type = MediaType.Video,
                            Url = "https://mock-video-url.com"
                        },
                        new Media
                        {
                            Type = MediaType.Photo,
                            Url = "https://mock-photo-url.com"
                        } 
                    },
                    Content = "Mock medias",
                    Url = "https://mock-url.com"
                }, "test");
            
            await _bot.FlushAsync();
        }

        private static User GetFirstConfiguredUser()
        {
            if (_config.IsFailure)
            {
                throw new Exception("Developer exception. Mock config should never be null");
            }

            return _config.Value.Users.FirstOrDefault();
        }
    }
}