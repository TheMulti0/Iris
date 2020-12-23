using System.Collections.Generic;
using System.Linq;

namespace IrisPoc
{
    internal class DataLayer : IDataLayer
    {
        private readonly Dictionary<UserPollRule, List<string>> _usersChatIds = new();
        
        public void Add(UserPollRule info, string chatId)
        {
            bool containsKey = _usersChatIds.ContainsKey(info);
            
            if (containsKey)
            {
                _usersChatIds[info].Add(chatId);
            }
            else
            {
                _usersChatIds[info] = new List<string>
                {
                    chatId
                };
            }
        }

        public void Remove(UserPollRule info, string chatId)
        {
            bool containsKey = _usersChatIds.ContainsKey(info);
            if (!containsKey)
            {
                return;
            }

            var usersChatId = _usersChatIds[info];
            
            if (usersChatId.Count > 1)
            {
                _usersChatIds[info] = usersChatId.Where(c => c == chatId).ToList();
            }
            else
            {
                _usersChatIds.Remove(info);
            }
        }

        public Dictionary<UserPollRule, List<string>> Get()
        {
            return _usersChatIds;
        }
    }
}