import json
import logging
from datetime import datetime

from aiokafka import AIOKafkaProducer

from updatesproducer.iupdatesproducer import IUpdatesProducer
from updatesproducer.updateapi.imedia import IMedia


class UpdatesProducer(IUpdatesProducer):
    def __init__(
            self,
            config):
        self.__producer = AIOKafkaProducer(bootstrap_servers=config['bootstrap_servers'])
        self.__config = config['updates']
        self.__logger = logging.getLogger(UpdatesProducer.__name__)

    @staticmethod
    def _json_converter(obj):
        if isinstance(obj, datetime):
            return obj.__str__()
        if isinstance(obj, IMedia):
            return obj.__dict__

    async def start(self):
        await self.__producer.start()

    async def send(self, update):
        self.__logger.info('Sending update %s to Kafka as JSON UTF-8 bytes', update.url)

        json_str = json.dumps(update.__dict__, default=self._json_converter)
        key_bytes = bytes(self.__config['key'], 'utf-8')
        update_bytes = bytes(json_str, 'utf-8')

        await self.__producer.send(
            self.__config['topic'],
            key=key_bytes,
            value=update_bytes,
            timestamp_ms=int(datetime.now().timestamp()))
