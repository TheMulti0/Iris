using System;
using Updates.Api;

namespace Updates.Watcher
{
    public interface IUpdatesWatcher
    {
        IObservable<Update> Updates { get; }
    }
}