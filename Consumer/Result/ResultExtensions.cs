using System;
using System.Threading.Tasks;

namespace Consumer
{
    public static class ResultExtensions
    {
        public static Result<TTarget> Map<TSource, TTarget>(
            this Result<TSource> result,
            Func<TSource, TTarget> valueGetter)
        {
            if (result.IsFailure)
            {
                return Result<TTarget>.Failure(result.Error);
            }

            try
            {
                var value = valueGetter(result.Value);
                return Result<TTarget>.Success(value);
            }
            catch (Exception e)
            {
                return Result<TTarget>.Failure(e.Message);
            }
        }

        public static Task DoAsync<T>(
            this Result<T> result,
            Func<T, Task> asyncFunc)
        {
            return result.IsSuccess
                ? asyncFunc(result.Value) 
                : Task.CompletedTask;
        }
    }
}