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
        public Paged<Update> Get(int pageIndex, int pageSize)
        {
            Paged<UpdateEntity> paged = _repository
                .Get(pageIndex, pageSize);
            
            IEnumerable<Update> content = paged.Content
                .Select(ToUpdate);
            
            return new Paged<Update>(content, paged);
        }

        private static Update ToUpdate(UpdateEntity entity)
        {
            List<IMedia> media = new List<IMedia>()
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