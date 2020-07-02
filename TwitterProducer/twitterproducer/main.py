import json

from twitterproducer.userlatestupdatetimerepository import UserLatestUpdateTimeRepository
from twitterproducer.mongodbconfig import MongoDbConfig
from twitterproducer.tweetsproducer import TweetsProducer
from twitterproducer.topicproducerconfig import TopicProducerConfig

appsettings = json.load(open('appsettings.json'))

repository = UserLatestUpdateTimeRepository(
    MongoDbConfig(appsettings['mongodb'])
)

tweets_producer = TweetsProducer(
    TopicProducerConfig(appsettings['tweets_producer']),
    repository)

tweets_producer.start()
