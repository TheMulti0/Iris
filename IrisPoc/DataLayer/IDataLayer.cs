using System.Collections.Generic;

namespace IrisPoc
{
    internal interface IDataLayer
    {
        Dictionary<UserPollRule, List<string>> Get();

        void Add(UserPollRule info, string chatId);
        
        void Remove(UserPollRule info, string chatId);
    }
}