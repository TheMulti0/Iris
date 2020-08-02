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
        private static TelegramSender _sender;

        [ClassInitialize]
        public static Task Initialize(TestContext context)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(
                builder => builder.AddTestsLogging(context));

            _config = Result<TelegramConfig>.Failure("Default config");
            
            var configsProvider = new FileConfigsProvider();
            configsProvider.Configs.Subscribe(result => _config = result);

            _sender = new TelegramSender(
                configsProvider,
                new TelegramBotClientProvider(loggerFactory.CreateLogger<TelegramBotClientProvider>()), 
                new MessageSender(loggerFactory.CreateLogger<MessageSender>()),
                loggerFactory.CreateLogger<TelegramSender>());

            return Task.Delay(500); // Wait for JSON config to be read
        }
        
        [TestMethod]
        public Task TestText()
        {
            User user = GetFirstConfiguredUser();

            return _sender.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Content = "Mock update"
                });
        }
        
        [TestMethod]
        public Task TestLongText()
        {
            User user = GetFirstConfiguredUser();

            var content = "";
            for (int i = 0; i < 5000; i++)
            {
                content += $"{i} \n";
            }

            return _sender.SendAsync(
                new Update
                {
                    AuthorId = user.UserName,
                    Content = content
                });
        }
        
        [TestMethod]
        public Task TestTextWithUrl()
        {
            User user = GetFirstConfiguredUser();

            return _sender.SendAsync(
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

            return _sender.SendAsync(
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
        }
        
        [TestMethod]
        public Task TestPhotoWithDetails()
        {
            User user = GetFirstConfiguredUser();

            return _sender.SendAsync(
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
        }
        
        [TestMethod]
        public Task TestPhotoWithLongText()
        {
            User user = GetFirstConfiguredUser();

            var content = "";
            for (int i = 0; i < 5000; i++)
            {
                content += $"{i} \n";
            }

            return _sender.SendAsync(
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
        }
        
        [TestMethod]
        public Task TestVideo()
        {
            User user = GetFirstConfiguredUser();

            return _sender.SendAsync(
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
        }
        
        [TestMethod]
        public Task TestVideoWithDetails()
        {
            User user = GetFirstConfiguredUser();

            return _sender.SendAsync(
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
        }
        
        [TestMethod]
        public Task TestMultipleMediaWithDetails()
        {
            User user = GetFirstConfiguredUser();

            return _sender.SendAsync(
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