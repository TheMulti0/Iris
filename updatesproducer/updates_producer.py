import json
import logging
from datetime import datetime

from kafka import KafkaProducer

from updatesproducer.iupdatesproducer import IUpdatesProducer
from updatesproducer.updates_producer_config import UpdatesProducerConfig
from updatesproducer.updateapi.imedia import IMedia
from updatesproducer.updateapi.video import Video
from updatesproducer.updateapi.video_downloader import VideoDownloader


class UpdatesProducer(IUpdatesProducer):
    def __init__(
            self,
            config: UpdatesProducerConfig,
            video_downloader: VideoDownloader):
        self.__config = config
        self.__producer = KafkaProducer(bootstrap_servers=config.bootstrap_servers)
        self.__video_downloader = video_downloader
        self.__logger = logging.getLogger(UpdatesProducer.__name__)

    @staticmethod
    def _json_converter(obj):
        if isinstance(obj, datetime):
            return obj.__str__()
        if isinstance(obj, IMedia):
            return obj.__dict__

    def send(self, update):
        self._download_lowres_videos(update)

        self.__logger.info('Sending update %s to Kafka as JSON UTF-8 bytes', update.url)

        json_str = json.dumps(update.__dict__, default=self._json_converter)
        key_bytes = bytes(self.__config.key, 'utf-8')
        update_bytes = bytes(json_str, 'utf-8')

        self.__producer.send(
            self.__config.topic,
            key=key_bytes,
            value=update_bytes,
            timestamp_ms=int(datetime.now().timestamp()))

    def _download_lowres_videos(self, update):
        if update.should_redownload_video:
            # Find all of the old lowres videos that this update has (if any)
            lowres_videos = list(filter(
                lambda m: isinstance(m, Video),
                update.media))

            if len(lowres_videos) != 0:
                for lowres_video in lowres_videos:
                    self.__video_downloader.download_video(update, lowres_video)
