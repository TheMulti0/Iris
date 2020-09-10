from datetime import datetime

from youtube_dl import YoutubeDL

from updatesproducer.updateapi.media import Media
from updatesproducer.updateapi.mediatype import MediaType
from updatesproducer.updateapi.update import Update
from youtubeproducer.videos.video import Video

YOUTUBE_BASE_URL = 'https://www.youtube.com'
ISO8601 = datetime.now().replace(microsecond=0).isoformat()

class UpdateFactory:
    @staticmethod
    def to_update(video: Video):
        url = f'{YOUTUBE_BASE_URL}/watch?v={video.video_id}'

        ydl_opts = {
            'format': 'best',
            'quiet': True
        }

        with YoutubeDL(ydl_opts) as ydl:
            info = ydl.extract_info(url, download=False)

            thumbnail = video.thumbnails.get('high')
            if thumbnail is None:
                thumbnail = video.thumbnails.get('default')

            return Update(
                content=f'{video.title}\n{video.description}',
                author_id=video.channelId,
                creation_date=datetime.strptime(video.publishedAt, "%Y-%m-%dT%H:%M:%SZ"),
                url=url,
                media=[
                    # Downloaded later by VideoDownloader
                    Media(
                        info['url'],
                        MediaType.Video,
                        thumbnail['url'],
                        info['duration'],
                        info['width'],
                        info['height']
                    )
                ],
                repost=False,
                should_redownload_video=False
            )
