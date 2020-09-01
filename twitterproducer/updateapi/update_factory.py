import re

from tweepy import Status

from updatesproducer.updateapi.update import Update
from twitterproducer.updateapi.media_factory import MediaFactory

TWITTER_BASE_URL = 'https://www.twitter.com'


class UpdateFactory:
    @staticmethod
    def to_update(tweet: Status):
        user_id = tweet.user.screen_name

        return Update(
            # Replace pic.twitter.com/232dssad links with nothing
            content=UpdateFactory.sub([r'pic.twitter.com/\S+'], '', tweet.full_text),
            author_id=user_id,
            creation_date=tweet.created_at,
            url=f'{TWITTER_BASE_URL}/{user_id}/status/{tweet.id_str}',
            media=MediaFactory.to_media(tweet),
            repost=tweet.retweeted,
            should_redownload_video=False
        )

    @staticmethod
    def sub(patterns, replacement, text):
        newest_text = text
        for p in patterns:
            newest_text = re.sub(p, replacement, newest_text)

        return newest_text
