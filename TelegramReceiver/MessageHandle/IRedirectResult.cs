namespace TelegramReceiver
{
    public interface IRedirectResult
    {
        public Route? Route { get; }

        public Context Context { get; }
    }
}