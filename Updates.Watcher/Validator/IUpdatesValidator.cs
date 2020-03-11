using System;

namespace Updates.Watcher
{
    public interface IUpdatesValidator
    {
        IObservable<(long updateId, long authorId)> SentUpdates { get; }
        
        bool WasUpdateSent(long updateId, long authorId);

        void UpdateSent(long updateId, long authorId);
    }
}