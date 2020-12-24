using Common;

namespace PollRulesManager
{
    public interface IPollRequestsProducer
    {
        public void SendPollRequest(PollRequest request);
    }
}