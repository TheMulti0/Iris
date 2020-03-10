namespace ProducerApi
{
    public interface IUser
    {
        long Id { get; }

        string Name { get; }

        string DisplayName { get; }

        string Url { get; }
    }
}