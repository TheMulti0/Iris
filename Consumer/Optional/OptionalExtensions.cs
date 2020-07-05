using System;
using System.Threading.Tasks;

namespace Consumer
{
    public static class OptionalExtensions
    {
        public static Task DoAsync<T>(
            this Optional<T> optional,
            Func<T, Task> asyncFunc)
        {
            return optional.HasValue
                ? asyncFunc(optional.Value)
                : Task.CompletedTask;
        }
    }
}