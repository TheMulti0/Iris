using System;

namespace ProducerApi
{
    public interface IUpdate
    {
        long Id { get; }

        string Message { get; }

        IUser Author { get; }

        DateTime CreatedAt { get; }

        string Url { get; }
    }
}