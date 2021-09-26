using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.Net;
using TdLib;
using TelegramSender;

namespace TelegramClient.Tests
{
    [TestClass]
    public class TelegramClientTests
    {
        private static ITelegramClient _client;
        private static long _chatId;

        private readonly VideoExtractor _videoExtractor = new(new VideoExtractorConfig());
        private readonly HighQualityVideoExtractor _hqExtractor = new(new TelegramSender.VideoExtractorConfig
        {
            DownloadOnly = true
        });

        [ClassInitialize]
        public static async Task Initialize(TestContext context)
        {
            var rootConfig = new ConfigurationBuilder().AddUserSecrets<TelegramClientTests>().Build();

            var config = rootConfig.GetSection("TelegramClientConfig").Get<TelegramClientConfig>();
            var factory = new TelegramClientFactory(config, NullLoggerFactory.Instance);
            
            _client = await factory.CreateAsync();
            
            var chatId = rootConfig.GetValue<long>("TelegramClientTestChatId");
            _chatId = (await _client.GetChatAsync(chatId)).Id;
        }

        [TestMethod]
        public async Task TestRecyclingFile()
        {
            const string filePath = "myfile.txt";
            await File.WriteAllTextAsync(filePath, "my text");

            await _client.SendMessageAsync(
                _chatId,
                new TdApi.InputMessageContent.InputMessageDocument
                {
                    Document = new InputRecyclingLocalFile(filePath)
                });
            
            Assert.IsFalse(File.Exists(filePath));
        }
        
        [DataTestMethod]
        [DataRow("test")]
        public async Task TestTextMessage(string text)
        {
            await TestSendMessage(new TdApi.InputMessageContent.InputMessageText
            {
                Text = new TdApi.FormattedText
                {
                    Text = text
                }
            });
        }
        
        [DataTestMethod]
        [DataRow("test")]
        public async Task TestReplyTextMessage(string text)
        {
            var content = new TdApi.InputMessageContent.InputMessageText
            {
                Text = new TdApi.FormattedText
                {
                    Text = text
                }
            };
            TdApi.Message message = await _client.SendMessageAsync(
                _chatId,
                content);
            
            Assert.IsNotNull(message);
            
            TdApi.Message message2 = await _client.SendMessageAsync(
                _chatId,
                content,
                replyToMessageId: message.Id);
            
            Assert.IsNotNull(message2);
        }
        
        [DataTestMethod]
        [DataRow("https://images.unsplash.com/photo-1529736576495-1ed4a29ca7e1?ixlib=rb-1.2.1&ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&auto=format&fit=crop&w=752&q=80")]
        public async Task TestPhotoMessage(string photoUrl)
        {
            await TestSendMessage(
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
        public async Task TestDirectVideoMessage(string directVideoUrl)
        {
            await TestSendMessage(
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
        public async Task TestLightStreamVideoMessage(string toBeExtractedStreamVideoUrl)
        {
            await TestStreamVideoMessage(toBeExtractedStreamVideoUrl);
        }
        
        [DataTestMethod]
        [DataRow("https://www.facebook.com/396697410351933/videos/3732920580089470")]
        //[DataRow("https://www.youtube.com/watch?v=78g-Qsoe3po")]
        // Should take a long time because this test is supposed to download and upload heavy video streams to Telegram
        public async Task TestHeavyStreamVideoMessage(string toBeExtractedStreamVideoUrl)
        {
            await TestStreamVideoMessage(toBeExtractedStreamVideoUrl);
        }

        [DataTestMethod]
        [DataRow("https://www.facebook.com/396697410351933/videos/3732920580089470")]
        //[DataRow("https://www.youtube.com/watch?v=78g-Qsoe3po")]
        // Should take a long time because this test is supposed to download and upload heavy video files to Telegram
        public async Task TestPreDownloadedVideoMessage(string toBeDownloadedVideoUrl)
        {
            var videoItem = await _hqExtractor.ExtractAsync(toBeDownloadedVideoUrl);

            var inputMessageContent = videoItem.ToInputMessageContent(new TdApi.FormattedText());

            await TestSendMessage(inputMessageContent);
        }
        
        [DataTestMethod]
        [DataRow("https://www.youtube.com/watch?v=78g-Qsoe3po")]
        // Should take a long time because this test is supposed to download and upload heavy video files to Telegram
        public async Task TestPreDownloadedVideoMessageWithRemoteThumbnail(string videoUrl)
        {
            var remote = await _videoExtractor.ExtractVideo(videoUrl);
            var hq = await _hqExtractor.ExtractAsync(videoUrl, downloadThumbnail: false);

            var inputMessageContent = GetInputMessageContent(hq, remote.ThumbnailUrl);

            await TestSendMessage(inputMessageContent);
        }

        private static TdApi.InputMessageContent GetInputMessageContent(IMediaItem mediaItem, string thumbnailUrl)
        {
            switch (mediaItem)
            {
                case VideoItem i:
                    i = i with { ThumbnailUrl = thumbnailUrl };
                    return i.ToInputMessageContent(new TdApi.FormattedText());
                case LocalVideoItem c:
                    c = c with { ThumbnailUrl = thumbnailUrl };
                    return c.ToInputMessageContent(new TdApi.FormattedText());
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

            var inputThumbnail = new TdApi.InputThumbnail
            {
                Width = extractedVideo.Width ?? 0,
                Height = extractedVideo.Height ?? 0,
                Thumbnail = new TdApi.InputFile.InputFileRemote
                {
                    Id = extractedVideo.ThumbnailUrl
                }
            };
            return new TdApi.InputMessageContent.InputMessageVideo
            {
                Video = new TdApi.InputFile.InputFileRemote
                {
                    Id = extractedVideo.Url
                },
                Thumbnail = extractedVideo.ThumbnailUrl != null 
                    ? inputThumbnail 
                    : null
            };
        }

        private static async Task TestSendMessage(TdApi.InputMessageContent messageContent)
        {
            void LogFileUploadProgress(FileUploadProgress progress)
            {
                double percentage = (double) progress.UploadedSize / progress.TotalSize * 100;
                Debug.WriteLine($"Uploaded {percentage:F3}%");
            }

            TdApi.Message message = await _client.SendMessageAsync(
                _chatId,
                messageContent,
                progress: LogFileUploadProgress);

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