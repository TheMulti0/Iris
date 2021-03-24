using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UpdatesDb;

namespace DashboardApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class FeedsController : ControllerBase
    {
        private readonly IFeedsRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedsController(
            IFeedsRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<FeedEntity> Get()
        {
            var user = await _userManager.GetUserAsync(User);
            
            return await _repository.GetAsync(user.Id);
        }
    }
}