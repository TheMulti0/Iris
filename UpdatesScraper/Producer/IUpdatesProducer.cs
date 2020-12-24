using Common;

namespace UpdatesScraper
{
    public interface IUpdatesProducer
    {
        public void SendUpdate(Update update);
    }
}