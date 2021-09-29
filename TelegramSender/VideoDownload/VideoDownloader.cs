using System;
using System.Threading;
using System.Threading.Tasks;
using FFMpegCore;
using TelegramClient;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace TelegramSender
{
    public class VideoDownloader
    {
        private readonly YoutubeDL _youtubeDl;
        private readonly string _cookiesFileName;
        private readonly Random _random;

        public VideoDownloader(YoutubeDL youtubeDl, string cookiesFileName)
        {
            _youtubeDl = youtubeDl;
            _cookiesFileName = cookiesFileName;
            _random = new Random();
        }

        public async Task<LocalVideoItem> DownloadAsync(
            string url,
            bool downloadThumbnail = true,
            CancellationToken ct = default)
        {
            string videoPath = await DownloadVideo(url, ct);
            if (videoPath == null)
            {
                return null;
            }
            
            IMediaAnalysis analysis = await FFProbe.AnalyseAsync(videoPath);

            string thumbnailPath = null;
            if (downloadThumbnail)
            {
                thumbnailPath = await GetThumbnail(
                    videoPath,
                    (int) analysis.Duration.TotalMilliseconds);    
            }

            return new LocalVideoItem(videoPath, thumbnailPath, analysis);
        }

        private async Task<string> DownloadVideo(string url, CancellationToken ct)
        {
            var overrideOptions = new OptionSet
            {
                Output = InputRemoteStream.CreateUniqueFilePath(),
                Cookies = _cookiesFileName
            };
            RunResult<string> result = await _youtubeDl.RunVideoDownload(
                url,
                overrideOptions: overrideOptions,
                ct: ct);

            if (result.Success)
            {
                return result.Data;
            }
            
            string message = string.Join('\n', result.ErrorOutput);
            throw new YoutubeDlException(message);
        }

        private async Task<string> GetThumbnail(string videoPath, int durationMilli)
        {
            var thumbnailPath = $"{videoPath}.png";

            TimeSpan randomCaptureTime = TimeSpan.FromMilliseconds(
                _random.Next(durationMilli));
            
            bool success = await FFMpeg.SnapshotAsync(
                videoPath,
                thumbnailPath,
                captureTime: randomCaptureTime);
            
            return success 
                ? thumbnailPath 
                : null;
        }
    }
}