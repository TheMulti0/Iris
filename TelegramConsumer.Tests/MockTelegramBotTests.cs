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
                loggerFactory.CreateLogger<TelegramBot>(),
                loggerFactory.CreateLogger<MessageSender>());
        }
        
        [TestMethod]
        public Task TestText()
        {
            User user = GetFirstConfiguredUser();

            return _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Content = "Mock update"
                });
        }
        
        [TestMethod]
        public Task TestTextWithUrl()
        {
            User user = GetFirstConfiguredUser();

            return _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Content = "Mock update",
                    Url = "https://mock-url.com"
                });
        }
        
        [TestMethod]
        public Task TestPhoto()
        {
            User user = GetFirstConfiguredUser();

            return _bot.SendAsync(
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
                });
        }
        
        [TestMethod]
        public Task TestPhotoWithDetails()
        {
            User user = GetFirstConfiguredUser();

            return _bot.SendAsync(
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
                });
        }
        
        [TestMethod]
        public Task TestVideo()
        {
            User user = GetFirstConfiguredUser();

            return _bot.SendAsync(
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
                });
        }
        
        [TestMethod]
        public Task TestVideoWithDetails()
        {
            User user = GetFirstConfiguredUser();

            return _bot.SendAsync(
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
                });
        }
        
        [TestMethod]
        public Task TestMultipleMediaWithDetails()
        {
            User user = GetFirstConfiguredUser();

            return _bot.SendAsync(
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
                });
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