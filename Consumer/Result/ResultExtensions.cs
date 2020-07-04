using System;

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
                TTarget value = valueGetter(result.Value);
                return Result<TTarget>.Success(value);
            }
            catch (Exception e)
            {
                return Result<TTarget>.Failure(e.Message);
            }
        }
    }
}