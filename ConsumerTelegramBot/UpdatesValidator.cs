using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace ConsumerTelegramBot
{
    internal class UpdatesValidator : IUpdatesValidator
    {
        private readonly ConcurrentDictionary<long, List<long>> _operatedPosts;
        private readonly Subject<(long updateId, long authorId)> _sentUpdates;

        public IObservable<(long updateId, long authorId)> SentUpdates => _sentUpdates;

        public UpdatesValidator()
        {
            _operatedPosts = new ConcurrentDictionary<long, List<long>>();
            _sentUpdates = new Subject<(long postId, long userId)>();
        }

        public bool WasUpdateSent(long updateId, long authorId)
        {
            return _operatedPosts.ContainsKey(updateId) &&
                   _operatedPosts[updateId].Any(aid => aid == authorId);
        }

        public void UpdateSent(long updateId, long authorId)
        {
            _sentUpdates.OnNext((updateId, authorId));
            
            if (_operatedPosts.ContainsKey(updateId))
            {
                _operatedPosts[updateId].Add(authorId);
            }
            else
            {
                AddPostAndUser(updateId, authorId);
            }
        }

        private void AddPostAndUser(long postId, long userId)
        {
            bool tryAdd = _operatedPosts.TryAdd(
                postId,
                new List<long>
                {
                    userId
                });

            if (!tryAdd)
            {
                throw new InvalidOperationException($"Cannot add user #{userId} to post #{postId}");
            }
        }
    }
}