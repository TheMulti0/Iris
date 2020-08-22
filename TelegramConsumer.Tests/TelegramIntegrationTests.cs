using System;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelegramConsumer.Tests
{
    [TestClass]
    public class TelegramIntegrationTests
    {
        private const string VideoUrl = "http://www.sample-videos.com/video123/mp4/720/big_buck_bunny_720p_1mb.mp4";
        private const string PhotoUrl = "https://www.creare.co.uk/wp-content/uploads/2016/02/google-1018443_1920.png";
        private static Result<TelegramConfig> _config;
        private static TelegramBot _bot;

        [ClassInitialize]
        public static Task Initialize(TestContext context)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(
                builder => builder.AddTestsLogging(context));

            _config = Result<TelegramConfig>.Failure("Default config");
            
            var configsProvider = new FileConfigProvider();
            configsProvider.Configs.Subscribe(result => _config = result);

            _bot = new TelegramBot(
                configsProvider,
                new TelegramBotClientProvider(loggerFactory.CreateLogger<TelegramBotClientProvider>()),
                loggerFactory);

            return Task.Delay(1000); // Wait for JSON config to be read
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
                });
            
            await _bot.WaitForCompleteAsync();
        }
        
        [TestMethod]
        public async Task TestLongText()
        {
            User user = GetFirstConfiguredUser();

            var content = "";
            for (int i = 0; i < 5000; i++)
            {
                content += $"{i} \n";
            }

            await _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Content = content
                });
            
            await _bot.WaitForCompleteAsync();
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
                });
            
            await _bot.WaitForCompleteAsync();
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
                            Url = PhotoUrl
                        } 
                    }
                });
            
            await _bot.WaitForCompleteAsync();
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
                            Url = PhotoUrl
                        } 
                    },
                    Content = "Mock photo",
                    Url = "https://mock-url.com"
                });
            
            await _bot.WaitForCompleteAsync();
        }
        
        [TestMethod]
        public async Task TestPhotoWithLongText()
        {
            User user = GetFirstConfiguredUser();

            var content = "";
            for (int i = 0; i < 5000; i++)
            {
                content += $"{i} \n";
            }

            await _bot.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Content = content,
                    Media = new []
                    {
                        new Media
                        {
                            Type = MediaType.Photo,
                            Url = PhotoUrl
                        }
                    }
                });
            
            await _bot.WaitForCompleteAsync();
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
                            Url = VideoUrl
                        } 
                    }
                });
            
            await _bot.WaitForCompleteAsync();
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
                            Url = VideoUrl
                        } 
                    },
                    Content = "Mock video",
                    Url = "https://mock-url.com"
                });
            
            await _bot.WaitForCompleteAsync();
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
                            Url = VideoUrl
                        },
                        new Media
                        {
                            Type = MediaType.Photo,
                            Url = PhotoUrl
                        } 
                    },
                    Content = "Mock medias",
                    Url = "https://mock-url.com"
                });
            
            await _bot.WaitForCompleteAsync();
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