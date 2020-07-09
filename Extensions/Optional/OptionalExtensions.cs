using System;
using System.Threading.Tasks;

namespace Extensions
{
    public static class OptionalExtensions
    {
        public static bool ValueEqualsTo<T>(this Optional<T> optional, T expectedValue)
        {
            return optional.HasValue && optional.Value.Equals(expectedValue);
        }
        
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