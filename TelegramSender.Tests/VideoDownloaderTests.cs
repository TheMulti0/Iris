using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YoutubeDLSharp;

namespace TelegramSender.Tests
{
    [TestClass]
    public class VideoDownloaderTests
    {
        private readonly VideoDownloader _videoDownloader;

        public VideoDownloaderTests()
        {
            _videoDownloader = new VideoDownloader(
                new YoutubeDL(),
                null);
        }

        [DataTestMethod]
        [DataRow("https://www.facebook.com/396697410351933/videos/2725471531076700")]
        [DataRow("https://www.facebook.com/396697410351933/videos/454394315979515")]
        [DataRow("https://www.facebook.com/396697410351933/videos/184727739908065")]
        [DataRow("https://facebook.com/ayelet.benshaul.shaked/videos/230569472153183")]
        [DataRow("https://facebook.com/shirlypinto89/videos/968529917218882")]
        public async Task TestLightStreamVideoMessage(string toBeExtractedStreamVideoUrl)
        {
            LocalVideoItem downloaded = await _videoDownloader.DownloadAsync(toBeExtractedStreamVideoUrl);

            Assert.IsNotNull(downloaded.Url);
            Assert.IsTrue(File.Exists(downloaded.Url));
            File.Delete(downloaded.Url);
            
            if (downloaded.ThumbnailUrl != null)
            {
                Assert.IsTrue(File.Exists(downloaded.ThumbnailUrl));
                File.Delete(downloaded.ThumbnailUrl);
            }
        }
    }
}