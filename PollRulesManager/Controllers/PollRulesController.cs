using System.Collections.Generic;
using Common;
using Microsoft.AspNetCore.Mvc;

namespace PollRulesManager
{
    [ApiController]
    [Route("[controller]")]
    public class PollRulesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<UserPollRule> Get()
        {
            return new List<UserPollRule>();
        }
    }
}