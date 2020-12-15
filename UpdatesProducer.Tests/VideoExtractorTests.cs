using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdatesProducer.Tests
{
    
    [TestClass]
    public class VideoExtractorTests
    {
        [TestMethod]
        public async Task TestExtract()
        {
            const string url = "https://www.facebook.com/EretzNehederet.Keshet/videos/1254084181637623";

            var videoExtractor = new VideoExtractor(new VideoExtractorConfig
            {
                FormatRequest = "best"
            });
            var video = await videoExtractor.ExtractVideo(url);
            
            Assert.IsNotNull(video);
        }
        
        [TestMethod]
        public async Task TestExtractProd()
        {
            const string url = "https://www.facebook.com/EretzNehederet.Keshet/videos/1254084181637623";

            File.Delete("extract_video.py");
            File.Move("extract_video_prod.py", "extract_video.py");
            
            var videoExtractor = new VideoExtractor(new VideoExtractorConfig
            {
                FormatRequest = "best"
            });
            var video = await videoExtractor.ExtractVideo(url);
            
            Assert.IsNotNull(video);
        }
    }
}