using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Updates.Api;

namespace Iris.Bot
{
    internal class ChatsManager
    {
        private readonly string _fileName;
        private readonly object _fileLock = new object();
        public ConcurrentDictionary<long, long> ChatIds { get; }

        public ChatsManager(string fileName)
        {
            _fileName = fileName;
            var chats = JsonExtensions.Read<Chats>(fileName) ?? new Chats();
            List<long> chatIds = chats.ChatIds;
            ChatIds = new ConcurrentDictionary<long, long>(chatIds.Select(l => new KeyValuePair<long, long>(l, l)));
        }

        public void Add(long chatId)
        {
            if (ChatIds.TryAdd(chatId, chatId))
            {
                Save();
            } 
        }

        public void Remove(long chatId)
        {
            long c = chatId;
            if (ChatIds.TryRemove(chatId, out c))
            {
                Save();
            }
        }

        private void Save()
        {
            var chats = new Chats { ChatIds = ChatIds.Values.ToList() };
            lock (_fileLock)
            {
                JsonExtensions.Write(chats, _fileName);
            }
        }
    }
}