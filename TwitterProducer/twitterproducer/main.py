from kafka import KafkaProducer
import json
import time


# from twitterproducer.update import Update
#
# producer = KafkaProducer(bootstrap_servers='localhost:9092')
# i = 0
# while True:
#     i += 1
#     dict__ = Update(f'Message from Python #{i}').__dict__
#     msg = json.dumps(dict__)
#
#     producer.send(
#         'test-topic',
#         bytes(msg, 'utf-8'))
#
#     print(f'sent message {msg}')
#
#     time.sleep(5)


class TopicProducerConfig:
    topic: str
    bootstrap_servers: str
    update_interval_seconds: float

    def __init__(self, json_dict):
        self.__dict__ = json_dict


appsettings = json.load(open('appsettings.json'))


class UpdatesProducer:
    def __init__(self, config: TopicProducerConfig):
        self.__config = config

        self.__producer = KafkaProducer(
            bootstrap_servers=config.bootstrap_servers)


UpdatesProducer(
    TopicProducerConfig(appsettings['updates_topic_producer'])).
