namespace TelegramSender
{
    public class VideoDownloaderConfig
    {
        public string YoutubeDlPath { get; set; }
#if _WINDOWS
            = "youtube-dl.exe";
#endif

        public string FfBinariesFolder { get; set; } = "";
    }
}