using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using UserDataLayer;

namespace PollRulesManager
{
    [ApiController]
    [Route("[controller]")]
    public class PollRulesController : ControllerBase
    {
        private readonly ISavedUsersRepository _repository;

        public PollRulesController(ISavedUsersRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ValueTask<List<UserPollRule>> Get()
        {
            return _repository.GetAll()
                .ToAsyncEnumerable()
                .Select(ToPollRule)
                .ToListAsync();
        }

        private static UserPollRule ToPollRule(SavedUser user)
        {
            IOrderedEnumerable<ChatInfo> orderedByInterval = user.Chats.OrderBy(info => info.Interval);

            return new UserPollRule(
                user.User,
                orderedByInterval.FirstOrDefault()?.Interval);
        }
    }
}