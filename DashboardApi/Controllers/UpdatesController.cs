using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UpdatesDb;

namespace DashboardApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdatesController : ControllerBase
    {
        private readonly IUpdatesRepository _repository;

        public UpdatesController(IUpdatesRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<UpdateEntity> Get()
        {
            return _repository.Get();
        }
    }
}