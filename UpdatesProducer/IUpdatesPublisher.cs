using Common;

namespace UpdatesProducer
{
    public interface IUpdatesPublisher
    {
        public void Send(Update update);
    }
}