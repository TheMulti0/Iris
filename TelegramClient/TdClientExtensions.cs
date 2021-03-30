using System;
using System.Reactive.Linq;
using TdLib;

namespace TelegramClient
{
    internal static class TdClientExtensions
    {
        public static IObservable<TdApi.Update> OnUpdateReceived(this TdClient client)
        {
            return Observable.FromEventPattern<TdApi.Update>(
                action => client.UpdateReceived += action,
                action => client.UpdateReceived -= action)
                .Select(pattern => pattern.EventArgs);
        }
    }
}