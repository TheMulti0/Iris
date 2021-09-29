namespace TelegramSender
{
    public class VideoExtractorConfig
    {
        public string YoutubeDlPath { get; set; }
#if _WINDOWS
            = "youtube-dl.exe";
#else
            = "youtube-dl";
#endif

        public string FfBinariesFolder { get; set; } = "";

        public string CookiesFileName { get; set; }

        public bool DownloadOnly { get; set; }
    }
}