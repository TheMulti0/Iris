using System;

namespace Extensions
{
    public class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}