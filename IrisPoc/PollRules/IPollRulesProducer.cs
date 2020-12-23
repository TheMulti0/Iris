using System;
using System.Collections.Generic;

namespace IrisPoc
{
    internal interface IPollRulesProducer
    {
        IObservable<SetPollRule> SetUserPollRules { get; }
        
        IEnumerable<UserPollRule> GetPollRules();
    }
}