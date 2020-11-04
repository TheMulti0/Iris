import re

from tweepy import Status

from updatesproducer.updateapi.update import Update
from twitterproducer.updateapi.media_factory import MediaFactory

TWITTER_BASE_URL = 'https://www.twitter.com'


class UpdateFactory:
    @staticmethod
    def to_update(tweet: Status):
        user_id = tweet.user.screen_name

        try:
            # Throws exception if there is no such property as 'retweeted_status'
            text = tweet.retweeted_status.full_text
            retweeted = True
        except:
            text = tweet.full_text
            retweeted = False

        return Update(
            # Replace pic.twitter.com/******** links with nothing
            content=UpdateFactory.sub([r'pic.twitter.com/\S+'], '', text),
            author_id=user_id,
            creation_date=tweet.created_at,
            url=f'{TWITTER_BASE_URL}/{user_id}/status/{tweet.id_str}',
            media=MediaFactory.to_media(tweet),
            repost=retweeted,
            should_redownload_video=False
        )

    @staticmethod
    def sub(patterns, replacement, text):
        newest_text = text
        for p in patterns:
            newest_text = re.sub(p, replacement, newest_text)

        return newest_text
