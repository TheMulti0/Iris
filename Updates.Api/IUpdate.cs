using System;

namespace Updates.Api
{
    public interface IUpdate
    {
        long Id { get; }

        string Message { get; }

        IUser Author { get; }

        DateTime CreatedAt { get; }

        string Url { get; }
        
        string FormattedMessage { get; }
    }
}