import json
import re

import requests
from tweepy import Status

from updatesproducer.updateapi.update import Update
from twitterproducer.updateapi.media_factory import MediaFactory

TWITTER_BASE_DOMAIN = 'twitter.com'
TWITTER_BASE_DOMAIN_WWW = f'www.{TWITTER_BASE_DOMAIN}'

TWITTER_BASE_URL = f'https://{TWITTER_BASE_DOMAIN}'
TWITTER_BASE_URL_WWW = f'https://{TWITTER_BASE_DOMAIN_WWW}'

LINKUNSHORTEN_BASE_URL = 'https://linkunshorten.com/api'


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

        expanded_text = re.sub(
            r'https://t.co/\S+',
            lambda match: UpdateFactory.expand_url(match.group(0)),
            text)

        # Remove picture urls
        cleaned_text = UpdateFactory.sub(
            [
                r'pic.twitter.com/\S+',
                fr'{TWITTER_BASE_URL}/.+/status/\d+/photo/\d',
                fr'{TWITTER_BASE_URL_WWW}/.+/status/\d+/photo/\d'
            ],
            '',
            expanded_text)

        return Update(
            content=cleaned_text,
            author_id=user_id,
            creation_date=tweet.created_at,
            url=f'{TWITTER_BASE_URL_WWW}/{user_id}/status/{tweet.id_str}',
            media=MediaFactory.to_media(tweet),
            repost=retweeted,
            should_redownload_video=False
        )

    @staticmethod
    def expand_url(url):
        def is_expanded_url(u):
            return u is not None and \
                   u != url and \
                   u != TWITTER_BASE_DOMAIN and\
                   u != TWITTER_BASE_DOMAIN_WWW and\
                   u != TWITTER_BASE_URL

        try:
            response = requests.get(f'{LINKUNSHORTEN_BASE_URL}/link?url={url}').text
            response_json = json.loads(response)

            for k in ['redirectUrl', 'title']:
                u = response_json[k]

                if is_expanded_url(u):
                    return u
        except:
            return url

        return url

    @staticmethod
    def sub(patterns, replacement, text):
        newest_text = text
        for p in patterns:
            newest_text = re.sub(p, replacement, newest_text)

        return newest_text
