using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UpdatesDb;

namespace DashboardApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeedsController : ControllerBase
    {
        private readonly IFeedsRepository _repository;

        public FeedsController(IFeedsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<FeedEntity> Get()
        {
            return _repository.Get();
        }
    }
}