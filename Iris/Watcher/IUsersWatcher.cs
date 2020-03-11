using System;
using Updates.Api;

namespace Iris.Watcher
{
    internal interface IUsersWatcher
    {
        IObservable<IUpdate> Updates { get; }
    }
}