using System;
using System.Collections.Generic;

namespace IrisPoc
{
    internal interface IPollRulesProducer
    {
        IObservable<ChatPollRequest> ChatPollRequests { get; }
        
        IEnumerable<UserPollRule> GetPollRules();
    }
}