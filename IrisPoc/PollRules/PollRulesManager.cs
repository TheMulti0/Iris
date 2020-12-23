using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace IrisPoc
{
    internal class PollRulesManager : IChatPollRulesConsumer, IPollRulesProducer
    {
        private readonly Subject<ChatPollRequest> _setUserPollRuleRequests = new();
        public IObservable<ChatPollRequest> ChatPollRequests => _setUserPollRuleRequests;
        
        private readonly IDataLayer _dataLayer;

        public PollRulesManager(IDataLayer dataLayer)
        {
            _dataLayer = dataLayer;
        }

        public void Update(ChatPollRequest request)
        {
            Console.WriteLine($"Adding {request}");
            
            (Request type, UserPollRule info, string chatId) = request;
            if (type == Request.StartPoll)
            {
                Console.WriteLine($"Adding {request}");
                _dataLayer.Add(info, chatId);
            }
            else
            {
                Console.WriteLine($"Removing {request}");
                _dataLayer.Remove(info, chatId);
            }

            _setUserPollRuleRequests.OnNext(request);
        }

        public IEnumerable<UserPollRule> GetPollRules()
        {
            return _dataLayer.Get().Keys;
        }
    }
}