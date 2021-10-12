using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FFMpegCore;
using Scraper.Net;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace TelegramSender
{
    public class HighQualityVideoExtractor
    {
        private readonly VideoExtractorConfig _config;
        private readonly VideoDownloader _downloader;
        private readonly Scraper.Net.YoutubeDl.VideoExtractor _extractor;

        public HighQualityVideoExtractor(VideoExtractorConfig config)
        {
            _config = config;
            
            if (!string.IsNullOrEmpty(config.CookiesFileName))
            {
                config.CookiesFileName = CloneCookies(config);
            }
            
            GlobalFFOptions.Configure(new FFOptions
            {
                BinaryFolder = config.FfBinariesFolder
            });
            var youtubeDl = new YoutubeDL
            {
                YoutubeDLPath = config.YoutubeDlPath,
                FFmpegPath = GlobalFFOptions.GetFFMpegBinaryPath(),
                RestrictFilenames = true
            };
            var overrideOptions = new OptionSet
            {
                Cookies = config.CookiesFileName,
                CustomOptions = GetCustomYtDlpOptions()
            };
            
            _downloader = new VideoDownloader(youtubeDl, overrideOptions);
            _extractor = new Scraper.Net.YoutubeDl.VideoExtractor(youtubeDl, overrideOptions);
        }

        private IOption[] GetCustomYtDlpOptions()
        {
            return new IOption[]
            {
                new Option<string>(isCustom: true, "--compat-options")
                {
                    Value = "youtube-dl"
                },
                new Option<int>(isCustom: true, "--concurrent-fragments")
                {
                    Value = _config.ConcurrentFragments
                }
            };
        }

        private static string CloneCookies(VideoExtractorConfig config)
        {
            var destFileName = $"{config.CookiesFileName}_youtube-dl";
            
            File.Copy(config.CookiesFileName, destFileName, overwrite: true);
            
            return destFileName;
        }

        public async Task<IMediaItem> ExtractAsync(
            string url,
            bool downloadThumbnail = true,
            CancellationToken ct = default)
        {
            if (!_config.DownloadOnly)
            {
                long? freeSpace = GetFreeSpaceOnDrive();

                var remoteVideo = await _extractor.ExtractAsync(url, ct);
                long? fileSize = remoteVideo.FileSize;

                if (freeSpace == null || freeSpace * 1.5 < fileSize)
                {
                    return remoteVideo;
                }    
            }

            return await _downloader.DownloadAsync(url, downloadThumbnail, ct);
        }

        private static long? GetFreeSpaceOnDrive()
        {
            string rootDirectory = Directory.GetCurrentDirectory();
            
            DriveInfo currentDrive = DriveInfo.GetDrives()
                .FirstOrDefault(info => GetRoot(info.RootDirectory.FullName) == GetRoot(rootDirectory));
            
            return currentDrive?.AvailableFreeSpace;
        }

        private static string GetRoot(string path)
        {
            return path
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0];
        }
    }
}