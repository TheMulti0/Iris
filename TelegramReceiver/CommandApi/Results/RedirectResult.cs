namespace TelegramReceiver
{
    public class RedirectResult : IRedirectResult
    {
        public Route? Route { get; }
        public Context Context { get; }

        public RedirectResult(Route route)
        {
            Route = route;
        }

        public RedirectResult(Route route, Context context)
        {
            Route = route;
            Context = context;
        }
    }
}