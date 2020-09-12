from datetime import datetime

from youtube_dl import YoutubeDL

from updatesproducer.updateapi.update import Update
from updatesproducer.updateapi.video import Video
from youtubeproducer.videos.youtubevideo import YouTubeVideo

YOUTUBE_BASE_URL = 'https://www.youtube.com'
ISO8601 = datetime.now().replace(microsecond=0).isoformat()

class UpdateFactory:
    @staticmethod
    def to_update(video: YouTubeVideo):
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
                    Video(
                        url=info['url'],
                        thumbnail_url=thumbnail['url'],
                        duration_seconds=info['duration'],
                        width=info['width'],
                        height=info['height']
                    )
                ],
                repost=False,
                should_redownload_video=False
            )
