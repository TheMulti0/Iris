using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.AspNetCore.Mvc;
using UpdatesDb;

namespace DashboardApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdatesController : ControllerBase
    {
        private readonly IUpdatesRepository _repository;

        public UpdatesController(IUpdatesRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Update> Get()
        {
            return _repository
                .Get()
                .AsEnumerable()
                .Select(ToUpdate);
        }

        private static Update ToUpdate(UpdateEntity entity)
        {
            var media = new List<IMedia>()
                .Concat(entity.Audios)
                .Concat(entity.Photos)
                .Concat(entity.Videos)
                .ToList();

            return new Update
            {
                Author = entity.Author,
                Content = entity.Content,
                Screenshot = entity.Screenshot,
                Media = media,
                Url = entity.Url,
                CreationDate = entity.CreationDate,
                IsLive = entity.IsLive,
                IsReply = entity.IsReply,
                IsRepost = entity.IsRepost
            };
        }
    }
}