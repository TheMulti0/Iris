using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SubscriptionsDb;

namespace Dashboard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelegramSubscriptionsController : ControllerBase
    {
        private readonly IChatSubscriptionsRepository _repository;

        public TelegramSubscriptionsController(IChatSubscriptionsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IQueryable<SubscriptionEntity> Get()
        {
            return _repository.Get();
        }
    }
}