using Common;

namespace ScrapersDistributor
{
    public interface IJobsProducer
    {
        void SendJob(User user);
    }
}