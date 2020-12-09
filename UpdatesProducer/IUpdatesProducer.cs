using Common;

namespace UpdatesProducer
{
    public interface IUpdatesProducer
    {
        public void Send(Update update);
    }
}