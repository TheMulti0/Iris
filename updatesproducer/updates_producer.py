import json
import logging
from datetime import datetime

from kafka import KafkaProducer

from updatesproducer.iupdatesproducer import IUpdatesProducer
from updatesproducer.updateapi.imedia import IMedia


class UpdatesProducer(IUpdatesProducer):
    def __init__(
            self,
            config):
        self.__config = config
        self.__producer = KafkaProducer(bootstrap_servers=config['bootstrap_servers'])
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
        key_bytes = bytes(self.__config['key'], 'utf-8')
        update_bytes = bytes(json_str, 'utf-8')

        self.__producer.send(
            self.__config['topic'],
            key=key_bytes,
            value=update_bytes,
            timestamp_ms=int(datetime.now().timestamp()))
