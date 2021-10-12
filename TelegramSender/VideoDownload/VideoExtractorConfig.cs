namespace TelegramSender
{
    public class VideoExtractorConfig
    {
        public string YoutubeDlPath { get; set; }
#if _WINDOWS
            = "yt-dlp.exe";
#else
            = "yt-dlp-dl";
#endif

        public string FfBinariesFolder { get; set; } = "";

        public string CookiesFileName { get; set; }

        public bool DownloadOnly { get; set; }

        public int ConcurrentFragments { get; set; } = 4;
    }
}