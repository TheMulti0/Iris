namespace TelegramSender
{
    public class VideoDownloaderConfig
    {
        public string YoutubeDlPath { get; set; }
#if _WINDOWS
            = "youtube-dl.exe";
#else
            = "youtube-dl";
#endif

        public string FfBinariesFolder { get; set; } = "";

        public string CookiesFileName { get; set; }
    }
}