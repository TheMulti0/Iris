using System;

namespace Iris.Watcher
{
    public interface IUpdatesValidator
    {
        IObservable<(long updateId, long chatId)> SentUpdates { get; }
        
        bool WasUpdateSent(long updateId, long chatId);

        void UpdateSent(long updateId, long chatId);
    }
}