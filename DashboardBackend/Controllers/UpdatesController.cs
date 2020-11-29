using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DashboardBackend.Data;
using DashboardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("[action]")]
        public Task<int> Count()
        {
            return _repository.CountAsync();
        }

        [HttpGet]
        public Task<List<Update>> Get([FromQuery] PageSearchParams searchParams)
        {
            return _repository.Get(searchParams);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return StatusCode((int) HttpStatusCode.ServiceUnavailable); // Server error                
            }
        }
    }
}