using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Iris.Api;

namespace Updates.Watcher
{
    internal class UpdatePair
    {
        public long UpdateId { get; set; }
            
        public List<long> ChatIds { get; set; }
    }

    internal class Updates
    {
        public IEnumerable<UpdatePair> Pairs { get; set; }
    }
    
    public class JsonUpdateValidator : IUpdatesValidator
    {
        private readonly string _fileName;
        private readonly ConcurrentDictionary<long, List<long>> _operatedPosts;
        private readonly Subject<(long updateId, long chatId)> _sentUpdates;

        public IObservable<(long updateId, long chatId)> SentUpdates => _sentUpdates;

        public JsonUpdateValidator(string fileName)
        {
            _fileName = fileName;
            _operatedPosts = new ConcurrentDictionary<long, List<long>>();
            _sentUpdates = new Subject<(long postId, long chatId)>();

            Dictionary<long, List<long>> savedUpdates = GetSavedUpdates();
            
            foreach ((long updateId, List<long> chatIds) in savedUpdates)
            {
                foreach (long chatId in chatIds)
                {
                    Add(updateId, chatId, _operatedPosts);
                }
            }
        }

        private Dictionary<long, List<long>> GetSavedUpdates()
        {
            return JsonExtensions.Read<Updates>(_fileName)
                ?.Pairs
                ?.ToDictionary(pair => pair.UpdateId, pair => pair.ChatIds)
                ?? new Dictionary<long, List<long>>();
        }

        public bool WasUpdateSent(long updateId, long chatId)
        {
            return _operatedPosts.ContainsKey(updateId) &&
                   _operatedPosts[updateId].Any(cid => cid == chatId);
        }

        public void UpdateSent(long updateId, long chatId)
        {
            _sentUpdates.OnNext((updateId, chatId));

            Add(updateId, chatId, _operatedPosts);

            var updates = new Updates
            {
                Pairs = _operatedPosts.Select(pair => new UpdatePair
                {
                    UpdateId = pair.Key,
                    ChatIds = pair.Value
                }) 
            };
            JsonExtensions.Write(updates, _fileName);
        }

        private static void Add(
            long updateId,
            long chatId,
            IDictionary<long, List<long>> dictionary)
        {
            if (dictionary.ContainsKey(updateId))
            {
                dictionary[updateId].Add(chatId);
            }
            else
            {
                AddPostAndUser(updateId, chatId, dictionary);
            }
        }

        private static void AddPostAndUser(
            long updateId,
            long chatId,
            IDictionary<long, List<long>> dictionary)
        {
            bool tryAdd = dictionary.TryAdd(
                updateId,
                new List<long>
                {
                    chatId
                });

            if (!tryAdd)
            {
                throw new InvalidOperationException($"Cannot add author #{chatId} to update #{updateId}");
            }
        }
    }
}