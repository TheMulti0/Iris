using System.Linq;
using System.Threading.Tasks;
using Common;
using Extensions;
using MessagesManager;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TdLib;

namespace TelegramClient.Tests
{
    [TestClass]
    public class TelegramClientTests
    {
        private static ITelegramClient _client;
        private static long _chatId;

        private readonly VideoExtractor _videoExtractor = new(new VideoExtractorConfig());

        [ClassInitialize]
        public static async Task Initialize(TestContext context)
        {
            var rootConfig = new ConfigurationBuilder().AddUserSecrets<TelegramClientTests>().Build();

            var config = rootConfig.GetSection<TelegramClientConfig>("TelegramClientConfig");
            var factory = new TelegramClientFactory(config);
            
            _client = await factory.CreateAsync();
            
            var chatId = rootConfig.GetValue<long>("TelegramClientTestChatId");
            _chatId = (await _client.GetChatAsync(chatId)).Id;
        }
        
        [DataTestMethod]
        [DataRow("test")]
        public Task TestTextMessage(string text)
        {
            return TestSendMessage(new TdApi.InputMessageContent.InputMessageText
            {
                Text = new TdApi.FormattedText
                {
                    Text = text
                }
            });
        }
        
        [DataTestMethod]
        [DataRow("https://images.unsplash.com/photo-1529736576495-1ed4a29ca7e1?ixlib=rb-1.2.1&ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&auto=format&fit=crop&w=752&q=80")]
        public Task TestPhotoMessage(string photoUrl)
        {
            return TestSendMessage(
                new TdApi.InputMessageContent.InputMessagePhoto
                {
                    Photo = new TdApi.InputFile.InputFileRemote
                    {
                        Id = photoUrl
                    }
                });
        }
        
        [DataTestMethod]
        [DataRow("https://www.learningcontainer.com/wp-content/uploads/2020/05/sample-mp4-file.mp4")]
        public Task TestDirectVideoMessage(string directVideoUrl)
        {
            return TestSendMessage(
                new TdApi.InputMessageContent.InputMessageVideo
                {
                    Video = new TdApi.InputFile.InputFileRemote
                    {
                        Id = directVideoUrl
                    }
                });
        }
        
        [DataTestMethod]
        [DataRow("https://www.facebook.com/396697410351933/videos/2725471531076700")]
        [DataRow("https://www.facebook.com/396697410351933/videos/454394315979515")]
        [DataRow("https://www.facebook.com/396697410351933/videos/184727739908065")]
        [DataRow("https://facebook.com/ayelet.benshaul.shaked/videos/230569472153183")]
        [DataRow("https://facebook.com/shirlypinto89/videos/968529917218882")]
        public Task TestLightStreamVideoMessage(string toBeExtractedStreamVideoUrl)
        {
            return TestStreamVideoMessage(toBeExtractedStreamVideoUrl);
        }
        
        [DataTestMethod]
        [DataRow("https://www.facebook.com/396697410351933/videos/3732920580089470")]
        //[DataRow("https://www.youtube.com/watch?v=78g-Qsoe3po")]
        // Should take a long time because this test is supposed to download and upload heavy video streams to Telegram
        public Task TestHeavyStreamVideoMessage(string toBeExtractedStreamVideoUrl)
        {
            return TestStreamVideoMessage(toBeExtractedStreamVideoUrl);
        }

        [DataTestMethod]
        [DataRow("https://images.unsplash.com/photo-1529736576495-1ed4a29ca7e1?ixlib=rb-1.2.1&ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&auto=format&fit=crop&w=752&q=80", "https://www.learningcontainer.com/wp-content/uploads/2020/05/sample-mp4-file.mp4", "https://www.facebook.com/396697410351933/videos/3732920580089470")]
        public async Task TestMixedAlbumMessage(string photoUrl, string directVideoUrl, string toBeExtractedStreamVideoUrl)
        {
            var photoContent = new TdApi.InputMessageContent.InputMessagePhoto
            {
                Photo = new TdApi.InputFile.InputFileRemote
                {
                    Id = photoUrl
                }
            };

            var directVideoContent = new TdApi.InputMessageContent.InputMessageVideo
            {
                Video = new TdApi.InputFile.InputFileRemote
                {
                    Id = directVideoUrl
                }
            };
            
            TdApi.InputMessageContent.InputMessageVideo extractedVideoContent = await ExtractVideo(toBeExtractedStreamVideoUrl);

            await TestMessageAlbum(
                photoContent,
                directVideoContent,
                extractedVideoContent);
        }

        private async Task TestStreamVideoMessage(string toBeExtractedStreamVideoUrl)
        {
            TdApi.InputMessageContent.InputMessageVideo inputMessageContent = await ExtractVideo(toBeExtractedStreamVideoUrl);
            
            await TestSendMessage(inputMessageContent);
        }

        private async Task<TdApi.InputMessageContent.InputMessageVideo> ExtractVideo(string toBeExtractedStreamVideoUrl)
        {
            Video extractedVideo = await _videoExtractor.ExtractVideo(toBeExtractedStreamVideoUrl);

            return new TdApi.InputMessageContent.InputMessageVideo
            {
                Video = new TdApi.InputFile.InputFileRemote
                {
                    Id = extractedVideo.Url
                }
            };
        }

        private static async Task TestSendMessage(TdApi.InputMessageContent messageContent)
        {
            TdApi.Message message = await _client.SendMessageAsync(
                _chatId,
                messageContent);

            Assert.IsNotNull(message);
        }

        private static async Task TestMessageAlbum(params TdApi.InputMessageContent[] messageContent)
        {
            var messages = await _client.SendMessageAlbumAsync(
                _chatId,
                messageContent);

            Assert.IsNotNull(messages.FirstOrDefault());
        }
    }
}