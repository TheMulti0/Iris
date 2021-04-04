using System;
using System.Reactive.Linq;
using Remutable.Extensions;
using TdLib;

namespace TelegramClient
{
    internal static class TdExtensions
    {
        public static IObservable<TdApi.Update> OnUpdateReceived(this TdClient client)
        {
            return Observable.FromEventPattern<TdApi.Update>(
                action => client.UpdateReceived += action,
                action => client.UpdateReceived -= action)
                .Select(pattern => pattern.EventArgs);
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