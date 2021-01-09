namespace TelegramReceiver
{
    public class NoRedirectResult : IRedirectResult
    {
        public Route? Route { get; }
        
        public Context Context { get; }
    }
}