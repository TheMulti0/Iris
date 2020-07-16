using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Extensions
{
    public static class ObservableExtensions
    {
        public static IDisposable SubscribeAsync<T>(
            this IObservable<T> source,
            Func<T, Task> onNextAsync)
        {
            return source
                .SelectMany(value => Observable.FromAsync(() => onNextAsync(value)))
                .Subscribe();
        }
    }
}