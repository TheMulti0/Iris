namespace TelegramReceiver
{
    internal interface IPlatformUserIdExtractor
    {
        string Get(string userId);
    }
}