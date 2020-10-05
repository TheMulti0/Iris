using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.Data;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UpdatesConsumer;

namespace Dashboard.Controllers
{
    [ApiController]
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
    }
}