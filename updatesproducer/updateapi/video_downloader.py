from youtube_dl import YoutubeDL

from updatesproducer.updateapi.update import Update
from updatesproducer.updateapi.video import Video


def _try_download_video(url):
    ydl_opts = {
        'format': 'best',
        'quiet': True
    }

    with YoutubeDL(ydl_opts) as ydl:
        return ydl.extract_info(url, download=False)


class VideoDownloader:
    def __init__(self, logger):
        self.__logger = logger

    def download_video(self, update: Update, lowres_video):
        try:
            # Try downloading the video of this update
            video = _try_download_video(update.url)

            # Add new downloaded video
            update.media.append(Video(
                url=video['url']
            ))

            # Remove old lowres that was found
            update.media.remove(lowres_video)

        except:
            self.__logger.error(f'Failed to download video {lowres_video}', exc_info=1)
