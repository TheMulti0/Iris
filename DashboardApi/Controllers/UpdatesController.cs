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

        /// <summary>
        /// Used to bring a slice of items
        /// </summary>
        /// <param name="startIndex">Inclusive index of first item in slice</param>
        /// <param name="limit">Exclusive index of last item in slice (represents the size of the slice)</param>
        /// <returns></returns>
        [HttpGet]
        public Slice<Update> Get(int startIndex, int limit)
        {
            Slice<UpdateEntity> slice = _repository
                .Get(startIndex, limit);
            
            IEnumerable<Update> content = slice.Content
                .Select(ToUpdate);
            
            return new Slice<Update>(content, slice);
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