from youtube_dl import YoutubeDL

from updatesproducer.updateapi.update import Update
from updatesproducer.updateapi.video import Video


def download_video(url):
    download_video_with_params(url, {})


def download_video_with_params(url, params):
    ydl_opts = {
        'format': 'best',
        'quiet': True
    }.update(params)

    with YoutubeDL(ydl_opts) as ydl:
        return ydl.extract_info(url, download=False)


class VideoDownloader:
    def __init__(self, logger, params=None):
        if params is None:
            params = {}
        self.__logger = logger
        self.__params = params

    def download_video(self, update: Update, lowres_video):
        try:
            # Try downloading the video of this update
            info = download_video_with_params(update.url, self.__params)

            url = info.get('url')

            # url is not supplied in default format
            if url is None:
                # Take the url of the best format found
                url = info['formats'][-1]['url']

            video = Video(
                url=url,
                thumbnail_url=info.get('thumbnail'),
                duration_seconds=info.get('duration'),
                width=info.get('width'),
                height=info.get('height')
            )

            # Add new downloaded video
            update.media.append(video)

            # Remove old lowres that was found
            update.media.remove(lowres_video)

        except:
            self.__logger.exception(f'Failed to download video %s', lowres_video)
