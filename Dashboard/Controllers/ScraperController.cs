using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scraper.Net;

namespace Dashboard.Controllers
{
    [ApiController]
    [Route("[controller]/{platform}/{id}")]
    public class ScraperController : ControllerBase
    {
        private readonly IScraperService _service;

        public ScraperController(IScraperService service)
        {
            _service = service;
        }

        [HttpGet("author")]
        public Task<Author> GetAuthorAsync(string id, string platform, CancellationToken ct)
        {
            return _service.GetAuthorAsync(id, platform, ct);
        }

        [HttpGet("posts")]
        public IAsyncEnumerable<Post> GetPostsAsync(string id, string platform, CancellationToken ct)
        {
            return _service.GetPostsAsync(id, platform, ct);
        }
    }
}