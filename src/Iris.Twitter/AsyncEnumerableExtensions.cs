using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iris.Twitter
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<IAsyncEnumerable<T>> FlattenAsync<T>(this Task<IEnumerable<T>> task)
        {
            IEnumerable<T> enumerable = await task;
            
            return AsyncEnumerable.Create(
                token =>
                {
                    IEnumerator<T> enumerator = enumerable.GetEnumerator();

                    return AsyncEnumerator
                        .Create(
                            async () => await Task.FromResult(enumerator.MoveNext()),
                            () => enumerator.Current,
                            async () =>
                            {
                                enumerator.Dispose();
                                await Task.CompletedTask;
                            })
                        .WithCancellation(token);
                });
        }
    }
}