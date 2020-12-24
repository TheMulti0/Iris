using Common;

namespace UpdatesScraper
{
    public interface IUpdatesPublisher
    {
        public void Send(Update update);
    }
}