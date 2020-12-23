using Common;

namespace IrisPoc
{
    internal interface IUpdatesConsumer
    {
        void NewUpdate(Update update);
    }
}