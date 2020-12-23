using System;
using System.Threading.Tasks;

namespace IrisPoc
{
    internal static class Orchestrator
    {
        public static IChatPollRulesConsumer WireUp()
        {
            var dataLayer = new DataLayer();
            
            var requestManager = new PollRulesManager(dataLayer);
            var messageManager = new MessagesManager(dataLayer);
            var distributor = new ScrapersDistributor(requestManager);
            
            var scraper = new Scraper(messageManager, distributor);
            var sender = new Sender(messageManager);

            Task.Factory.StartNew(distributor.Work);

            return requestManager;
        }
   }
}