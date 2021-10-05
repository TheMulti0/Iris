using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Remutable.Extensions;
using TdLib;

namespace TelegramClient
{
    public static class TdExtensions
    {
        public static string Format(this IEnumerable<TdApi.InputMessageContent> contents)
        {
            return string.Join(
                " - ",
                contents.Select(content => content.Format()));
        }
        
        public static string Format(this TdApi.InputMessageContent content)
        {
            switch (content)
            {
                case TdApi.InputMessageContent.InputMessageVideo v:
                    return $"Video {v.Video.Format()}, Thumbnail {v.Thumbnail?.Thumbnail.Format()}, {v.Width}x{v.Height} size";
                
                case TdApi.InputMessageContent.InputMessageText t:
                    return $"Text {t.Text}";
                
                case TdApi.InputMessageContent.InputMessagePhoto p:
                    return $"Photo {p.Photo.Format()}, Thumbnail {p.Thumbnail?.Thumbnail.Format()}, {p.Width}x{p.Height} size";
                
                default:
                    return content.GetType().FullName;
            }
        }
        
        public static string Format(this TdApi.InputFile file)
        {
            switch (file)
            {
                case InputRemoteStream s:
                    return $"RemoteStream";
                    
                case InputRecyclingLocalFile rl:
                    return $"RecyclingLocal {rl.Path}";
                    
                case TdApi.InputFile.InputFileRemote r:
                    return $"Remote {r.Id}";
                    
                case TdApi.InputFile.InputFileLocal l:
                    return $"Local {l.Path}";
                    
                case TdApi.InputFile.InputFileId i:
                    return $"Id {i.Id}";
                    
                case TdApi.InputFile.InputFileGenerated g:
                    return $"Generated {g.OriginalPath}";
                
                default:
                    return file.GetType().FullName;    
            }
        }
        
        public static IObservable<TdApi.Update> OnUpdateReceived(this TdClient client)
        {
            return Observable.FromEventPattern<TdApi.Update>(
                action => client.UpdateReceived += action,
                action => client.UpdateReceived -= action)
                .Select(pattern => pattern.EventArgs);
        }
        
        public static bool HasCaption(this TdApi.InputMessageContent content, out TdApi.FormattedText caption)
        {
            switch (content)
            {
                case TdApi.InputMessageContent.InputMessageAnimation an:
                    caption = an.Caption;
                    break;
                
                case TdApi.InputMessageContent.InputMessageAudio au:
                    caption = au.Caption;
                    break;
                
                case TdApi.InputMessageContent.InputMessageDocument d:
                    caption = d.Caption;
                    break;
                
                case TdApi.InputMessageContent.InputMessagePhoto p:
                    caption = p.Caption;
                    break;
                
                case TdApi.InputMessageContent.InputMessageVideo v:
                    caption = v.Caption;
                    break;
                
                case TdApi.InputMessageContent.InputMessageVoiceNote von:
                    caption = von.Caption;
                    break;
                
                default:
                    caption = null;
                    return false;
            }

            return true;
        }

        public static TdApi.InputMessageContent WithCaption(this TdApi.InputMessageContent content, TdApi.FormattedText text)
        {
            switch (content)
            {
                case TdApi.InputMessageContent.InputMessageAnimation an:
                    return an.Remute(animation => animation.Caption, text);
                
                case TdApi.InputMessageContent.InputMessageAudio au:
                    return au.Remute(audio => audio.Caption, text);
                
                case TdApi.InputMessageContent.InputMessageDocument d:
                    return d.Remute(document => document.Caption, text);
                
                case TdApi.InputMessageContent.InputMessagePhoto p:
                    return new TdApi.InputMessageContent.InputMessagePhoto
                    {
                        Caption = text,
                        Height = p.Height,
                        Photo = p.Photo,
                        Thumbnail = p.Thumbnail,
                        Width = p.Width,
                        Ttl = p.Ttl,
                        AddedStickerFileIds = p.AddedStickerFileIds
                    };
                
                case TdApi.InputMessageContent.InputMessageVideo v:
                    return new TdApi.InputMessageContent.InputMessageVideo
                    {
                        Caption = text,
                        Duration = v.Duration,
                        Height = v.Height,
                        Thumbnail = v.Thumbnail,
                        Ttl = v.Ttl,
                        Video = v.Video,
                        Width = v.Width,
                        SupportsStreaming = v.SupportsStreaming,
                        AddedStickerFileIds = v.AddedStickerFileIds
                    };
                
                case TdApi.InputMessageContent.InputMessageVoiceNote von:
                    return von.Remute(note => note.Caption, text);
            }

            return content;
        }

        public static bool HasFile(this TdApi.InputMessageContent content, out TdApi.InputFile file)
        {
            switch (content)
            {
                case TdApi.InputMessageContent.InputMessageAnimation an:
                    file = an.Animation;
                    break;
                
                case TdApi.InputMessageContent.InputMessageAudio au:
                    file = au.Audio;
                    break;
                
                case TdApi.InputMessageContent.InputMessageDocument d:
                    file = d.Document;
                    break;
                
                case TdApi.InputMessageContent.InputMessagePhoto p:
                    file = p.Photo;
                    break;
                
                case TdApi.InputMessageContent.InputMessageVideo v:
                    file = v.Video;
                    break;
                
                case TdApi.InputMessageContent.InputMessageVideoNote vin:
                    file = vin.VideoNote;
                    break;
                
                case TdApi.InputMessageContent.InputMessageVoiceNote von:
                    file = von.VoiceNote;
                    break;
                
                default:
                    file = null;
                    return false;
            }
            
            return true;
        }

        public static bool HasInputFile<T>(this TdApi.InputMessageContent content, out T file) where T : TdApi.InputFile
        {
            if (content.HasFile(out TdApi.InputFile f) &&
                f is T s)
            {
                file = s;
                return true;
            }
            
            file = null;
            return false;
        }
        
        public static bool HasThumbnail(this TdApi.InputMessageContent content, out TdApi.InputThumbnail thumbnail)
        {
            switch (content)
            {
                case TdApi.InputMessageContent.InputMessageAnimation a:
                    thumbnail = a.Thumbnail;
                    break;
                
                case TdApi.InputMessageContent.InputMessageAudio au:
                    thumbnail = au.AlbumCoverThumbnail;
                    break;
                
                case TdApi.InputMessageContent.InputMessageDocument d:
                    thumbnail = d.Thumbnail;
                    break;
                
                case TdApi.InputMessageContent.InputMessagePhoto p:
                    thumbnail = p.Thumbnail;
                    break;
                
                case TdApi.InputMessageContent.InputMessageVideo v:
                    thumbnail = v.Thumbnail;
                    break;
                
                case TdApi.InputMessageContent.InputMessageVideoNote vin:
                    thumbnail = vin.Thumbnail;
                    break;
                
                default:
                    thumbnail = null;
                    return false;
            }
            
            return thumbnail != null;
        }

        public static TdApi.InputMessageContent WithFile(this TdApi.InputMessageContent content, TdApi.InputFile file)
        {
            switch (content)
            {
                case TdApi.InputMessageContent.InputMessageAnimation an:
                    return an.Remute(animation => animation.Animation, file);
                
                case TdApi.InputMessageContent.InputMessageAudio au:
                    return au.Remute(audio => audio.Audio, file);
                
                case TdApi.InputMessageContent.InputMessageDocument d:
                    return d.Remute(document => document.Document, file);
                
                case TdApi.InputMessageContent.InputMessagePhoto p:
                    return new TdApi.InputMessageContent.InputMessagePhoto
                    {
                        Caption = p.Caption,
                        Height = p.Height,
                        Photo = file,
                        Thumbnail = p.Thumbnail,
                        Width = p.Width,
                        Ttl = p.Ttl,
                        AddedStickerFileIds = p.AddedStickerFileIds
                    };
                
                case TdApi.InputMessageContent.InputMessageVideo v:
                    return new TdApi.InputMessageContent.InputMessageVideo
                    {
                        Caption = v.Caption,
                        Duration = v.Duration,
                        Height = v.Height,
                        Thumbnail = v.Thumbnail,
                        Ttl = v.Ttl,
                        Video = file,
                        Width = v.Width,
                        SupportsStreaming = v.SupportsStreaming,
                        AddedStickerFileIds = v.AddedStickerFileIds
                    };
                
                case TdApi.InputMessageContent.InputMessageVideoNote vin:
                    return vin.Remute(note => note.VideoNote, file);
                
                case TdApi.InputMessageContent.InputMessageVoiceNote von:
                    return von.Remute(note => note.VoiceNote, file);
            }

            return content;
        }
        
        public static TdApi.InputThumbnail WithFile(this TdApi.InputThumbnail thumbnail, TdApi.InputFile file)
        {
            return new()
            {
                Width = thumbnail.Width,
                Height = thumbnail.Height,
                Thumbnail = file
            };
        }
        
        public static async Task<DisposableMessageContent> WithInputRecyclingLocalFileAsync(this TdApi.InputMessageContent content, InputRecyclingLocalFile file)
        {
            var newContent = content.WithFile(file);
            
            var withoutThumbnail = new DisposableMessageContent(newContent, file);
            if (!content.HasThumbnail(out TdApi.InputThumbnail thumbnail))
            {
                return withoutThumbnail;
            }

            return await HandleThumbnail(newContent, file, thumbnail);
        }

        public static async Task<DisposableMessageContent> WithInputRemoteStreamAsync(this TdApi.InputMessageContent content, InputRemoteStream file)
        {
            TdApi.InputMessageContent newContent = content
                .WithFile(await file.CreateLocalInputFileAsync());

            var withoutThumbnail = new DisposableMessageContent(newContent, file);
            if (!newContent.HasThumbnail(out TdApi.InputThumbnail thumbnail))
            {
                return withoutThumbnail;
            }
            
            return await HandleThumbnail(newContent, file, thumbnail);
        }

        private static async Task<DisposableMessageContent> HandleThumbnail(
            TdApi.InputMessageContent content,
            object disposableFile,
            TdApi.InputThumbnail thumbnail)
        {
            switch (thumbnail.Thumbnail)
            {
                case InputRecyclingLocalFile r:
                    return new DisposableMessageContent(
                        content,
                        new AggregateAsyncDisposable(disposableFile, r));

                case InputRemoteStream s:
                    return await WithThumbnailAsync(content, disposableFile, thumbnail, s);
                
                case TdApi.InputFile.InputFileRemote r:
                    // Remote thumbnail must be downloaded if file is not remote
                    var stream = new InputRemoteStream(new RemoteFileStream(r.Id).GetStreamAsync);
                    
                    return await WithThumbnailAsync(content, disposableFile, thumbnail, stream);

                default:
                    return new DisposableMessageContent(
                        content.WithThumbnail(thumbnail),
                        new AggregateAsyncDisposable(disposableFile));
            }
        }

        private static async Task<DisposableMessageContent> WithThumbnailAsync(
            TdApi.InputMessageContent content,
            object disposableFile,
            TdApi.InputThumbnail thumbnail,
            InputRemoteStream thumbnailStream)
        {
            TdApi.InputThumbnail inputThumbnail = thumbnail
                .WithFile(await thumbnailStream.CreateLocalInputFileAsync());

            content = content.WithThumbnail(inputThumbnail);

            return new DisposableMessageContent(
                content,
                new AggregateAsyncDisposable(disposableFile, thumbnailStream));
        }

        public static TdApi.InputMessageContent WithThumbnail(this TdApi.InputMessageContent content, TdApi.InputThumbnail thumbnail)
        {
            switch (content)
            {
                case TdApi.InputMessageContent.InputMessageAnimation an:
                    return an.Remute(animation => animation.Thumbnail, thumbnail);
                
                case TdApi.InputMessageContent.InputMessageAudio au:
                    return au.Remute(audio => audio.AlbumCoverThumbnail, thumbnail);
                
                case TdApi.InputMessageContent.InputMessageDocument d:
                    return d.Remute(document => document.Thumbnail, thumbnail);
                
                case TdApi.InputMessageContent.InputMessagePhoto p:
                    return new TdApi.InputMessageContent.InputMessagePhoto
                    {
                        Caption = p.Caption,
                        Height = p.Height,
                        Photo = p.Photo,
                        Thumbnail = thumbnail,
                        Width = p.Width,
                        Ttl = p.Ttl,
                        AddedStickerFileIds = p.AddedStickerFileIds
                    };
                
                case TdApi.InputMessageContent.InputMessageVideo v:
                    return new TdApi.InputMessageContent.InputMessageVideo
                    {
                        Caption = v.Caption,
                        Duration = v.Duration,
                        Height = v.Height,
                        Thumbnail = thumbnail,
                        Ttl = v.Ttl,
                        Video = v.Video,
                        Width = v.Width,
                        SupportsStreaming = v.SupportsStreaming,
                        AddedStickerFileIds = v.AddedStickerFileIds
                    };
                
                case TdApi.InputMessageContent.InputMessageVideoNote vin:
                    return vin.Remute(note => note.Thumbnail, thumbnail);
            }

            return content;
        }

        public static bool HasUrl(this TdApi.InputFile file, out string url)
        {
            switch (file)
            {
                case TdApi.InputFile.InputFileRemote r when r.Id.StartsWith("http"):
                    url = r.Id;
                    return true;
                
                default:
                    url = "";
                    return false;
            }
        }
    }
}