import json
import logging

from twitterproducer.userlatestupdatetimerepository import UserLatestUpdateTimeRepository
from twitterproducer.mongodbconfig import MongoDbConfig
from twitterproducer.tweetsproducer import TweetsProducer
from twitterproducer.topicproducerconfig import TopicProducerConfig

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
