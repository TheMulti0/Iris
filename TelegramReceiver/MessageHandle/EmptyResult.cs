namespace TelegramReceiver
{
    public class EmptyResult : IRedirectResult
    {
        public Route? Route { get; }
        
        public Context Context { get; }
    }
}