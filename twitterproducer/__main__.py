import json
import logging

from producer.mongodbconfig import MongoDbConfig
from producer.userlatestupdatetimerepository import UserLatestUpdateTimeRepository

from twitterproducer.tweetsproducer import TweetsProducer
from producer.topicproducerconfig import TopicProducerConfig


def main():
    logging.basicConfig(
        format='[%(asctime)s] %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S %z',
        level=logging.DEBUG)

    appsettings = json.load(open('appsettings.json'))

    repository = UserLatestUpdateTimeRepository(
        MongoDbConfig(appsettings['mongodb']),
        logging.getLogger('UserLatestUpdateTimeRepository')
    )

    tweets_producer = TweetsProducer(
        TopicProducerConfig(appsettings['tweets_producer']),
        repository,
        logging.getLogger('UserLatestUpdateTimeRepository'))

    tweets_producer.start()


if __name__ == '__main__':
    main()
