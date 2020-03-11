using System;

namespace Iris.Watcher
{
    internal interface IUpdatesValidator
    {
        IObservable<(long updateId, long authorId)> SentUpdates { get; }
        
        bool WasUpdateSent(long updateId, long authorId);

        void UpdateSent(long updateId, long authorId);
    }
}