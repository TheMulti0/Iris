using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DashboardBackend.Data;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UpdatesConsumer;

namespace DashboardBackend.Controllers
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