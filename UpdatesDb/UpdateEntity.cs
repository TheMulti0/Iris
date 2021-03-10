using System;
using Common;
using MongoDB.Bson;

namespace UpdatesDb
{
    public record UpdateEntity : Update
    {
        public ObjectId Id { get; set; }

        public DateTime SaveDate { get; set; } = DateTime.Now;

        public UpdateEntity()
        {
        }
        
        public UpdateEntity(Update update)
        {
            Author = update.Author;
            Content = update.Content;
            Media = update.Media;
            Screenshot = update.Screenshot;
            Url = update.Url;
            CreationDate = update.CreationDate;
            IsLive = update.IsLive;
            IsReply = update.IsReply;
            IsRepost = update.IsRepost;
        }
    }
}