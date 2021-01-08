using System.Threading;
using System.Threading.Tasks;

namespace TelegramReceiver
{
    public interface INewCommand
    {
        Task<IRedirectResult> ExecuteAsync(CancellationToken token);
    }

    public interface IRedirectResult
    {
        public Route? Route { get; }

        public Context Context { get; }
    }

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

    public class EmptyResult : IRedirectResult
    {
        public Route? Route { get; }
        
        public Context Context { get; }
    }
}