namespace Extensions
{
    public interface IProducer<in T>
    {
        void Send(T item);
    }
}