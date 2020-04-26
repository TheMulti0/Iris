using System;
using Iris.Api;

namespace Updates.Watcher
{
    public interface IUpdatesWatcher
    {
        IObservable<Update> Updates { get; }
    }
}