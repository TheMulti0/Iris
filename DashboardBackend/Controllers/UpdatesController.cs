using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DashboardBackend.Data;
using DashboardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UpdatesConsumer;

namespace DashboardBackend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UpdatesController : Controller
    {
        private readonly IUpdatesRepository _repository;

        public UpdatesController(IUpdatesRepository repository)
        {
            _repository = repository;
        }

        [Route("count")]
        [HttpGet]
        public Task<int> Count()
        {
            return _repository.CountAsync();
        }

        [HttpGet]
        public Task<List<Update>> Get([FromQuery] PageSearchParams searchParams)
        {
            return _repository.Get(searchParams).ToListAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            // Update update = await _repository.Updates.FindAsync(id);
            // EntityEntry<Update> deleted = _repository.Updates.Remove(update);
            //
            // if (deleted.Entity.Idd == id)
            // {
            //     await _repository.SaveChangesAsync();
            //     
            //     return Ok();
            // }
            //
            // deleted.State = EntityState.Unchanged;
            return StatusCode(503); // Server error
        }
    }
}