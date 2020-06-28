using System;

namespace Extensions
{
    internal class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}