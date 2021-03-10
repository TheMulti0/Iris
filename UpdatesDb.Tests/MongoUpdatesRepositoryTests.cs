using System.Threading.Tasks;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDbGenericRepository;

namespace UpdatesDb.Tests
{
    [TestClass]
    public class MongoUpdatesRepositoryTests
    {
        private readonly IUpdatesRepository _repository;

        public MongoUpdatesRepositoryTests()
        {
            var config = new MongoDbConfig
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "test"
            };

            var mongoDbContext = new MongoDbContext(config.ConnectionString, config.DatabaseName);

            _repository = new MongoUpdatesRepository(mongoDbContext, config);
        }
        
        [TestMethod]
        public async Task TestAdd()
        {
            var updateEntity = new UpdateEntity
            {
                Content = "test"
            };
            await _repository.AddOrUpdateAsync(updateEntity);
        }
    }
}