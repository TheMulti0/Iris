using System;
using Iris.Api;

namespace Iris.Watcher
{
    public interface IUpdatesWatcher
    {
        IObservable<Update> Updates { get; }
    }
}