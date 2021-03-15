using System;
using System.Collections.Generic;
using Common;
using MongoDB.Bson;

namespace UpdatesDb
{
    public record UpdateEntity
    {
        public ObjectId Id { get; set; }

        public DateTime SaveDate { get; set; } = DateTime.Now;
        
        public string Content { get; init; }

        public User Author { get; init; }

        public DateTime? CreationDate { get; init; }

        public string Url { get; init; }

        public List<Video> Videos { get; init; } = new();
        
        public List<Photo> Photos { get; init; } = new();
        
        public List<Audio> Audios { get; init; } = new();

        public bool IsRepost { get; init; }

        public bool IsLive { get; set; }

        public bool IsReply { get; set; }
        
        public byte[] Screenshot { get; set; }

        public UpdateEntity()
        {
        }
        
        public UpdateEntity(Update update)
        {
            Author = update.Author;
            Content = update.Content;
            Screenshot = update.Screenshot;
            Url = update.Url;
            CreationDate = update.CreationDate;
            IsLive = update.IsLive;
            IsReply = update.IsReply;
            IsRepost = update.IsRepost;

            FillMedia(update);
        }

        private void FillMedia(Update update)
        {
            foreach (IMedia media in update.Media)
            {
                switch (media)
                {
                    case Audio a:
                        Audios.Add(a);
                        break;

                    case Photo p:
                        Photos.Add(p);
                        break;

                    case Video v:
                        Videos.Add(v);
                        break;
                }
            }
        }
    }
}