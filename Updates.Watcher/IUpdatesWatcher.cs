using System;
using Updates.Api;

namespace Updates.Watcher
{
    public interface IUpdatesWatcher
    {
        IObservable<IUpdate> Updates { get; }
    }
}