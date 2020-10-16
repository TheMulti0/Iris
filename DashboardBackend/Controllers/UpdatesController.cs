using System.Collections.Generic;
using System.Threading.Tasks;
using DashboardBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UpdatesConsumer;

namespace DashboardBackend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UpdatesController : ControllerBase
    {
        private readonly ApplicationDbContext _repository;

        public UpdatesController(ApplicationDbContext repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public Task<List<Update>> Get()
        {
            return _repository.Updates.ToListAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            Update update = await _repository.Updates.FindAsync(id);
            EntityEntry<Update> deleted = _repository.Updates.Remove(update);

            if (deleted.Entity.Idd == id)
            {
                await _repository.SaveChangesAsync();
                
                return Ok();
            }
            
            deleted.State = EntityState.Unchanged;
            return StatusCode(503); // Server error
        }
    }
}