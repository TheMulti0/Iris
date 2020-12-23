using System;
using Common;

namespace IrisPoc
{
    internal class Scraper
    {
        private readonly IUpdatesConsumer _manager;

        public Scraper(IUpdatesConsumer manager, ScrapersDistributor distributor)
        {
            _manager = manager;

            distributor.UserIds.Subscribe(OnNext);
        }

        private void OnNext(string userId)
        {
            Console.WriteLine($"Sending update for {userId}");
            
            _manager.NewUpdate(
                new Update
                {
                    Content = "Mock",
                    AuthorId = userId
                });
        }
    }
}