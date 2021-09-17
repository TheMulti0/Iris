namespace TelegramReceiver
{
    public class FeedsUserIdExtractor : IPlatformUserIdExtractor
    {
        public string Get(string userId)
        {
            return userId.StartsWith("http") 
                ? userId 
                : null;
        }
    }
}