using System;
using System.IO;
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
        private readonly VideoDownloaderConfig _config;
        private readonly YoutubeDL _youtubeDl;
        private readonly Random _random;

        public VideoDownloader(VideoDownloaderConfig config)
        {
            _config = config;
            if (!string.IsNullOrEmpty(_config.CookiesFileName))
            {
                var destFileName = $"{_config.CookiesFileName}_youtube-dl";
                File.Copy(_config.CookiesFileName, destFileName);
                _config.CookiesFileName = destFileName;
            }
            
            GlobalFFOptions.Configure(new FFOptions
            {
                BinaryFolder = _config.FfBinariesFolder
            });
            _youtubeDl = new YoutubeDL
            {
                YoutubeDLPath = _config.YoutubeDlPath,
                FFmpegPath = GlobalFFOptions.GetFFMpegBinaryPath(),
                RestrictFilenames = true
            };
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
                Cookies = _config.CookiesFileName
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