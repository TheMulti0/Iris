from tweepy import Status

from updatesproducer.updateapi.photo import Photo
from updatesproducer.updateapi.video import Video


class MediaFactory:
    @staticmethod
    def to_media(tweet: Status):
        tweet_media = MediaFactory.get_tweet_media(tweet)

        media = []
        for m in tweet_media:
            if m.get('type') == 'photo':
                https_url = m.get('media_url_https')

                media.append(Photo(
                    https_url if https_url is not None else m.get('media_url')
                ))
            else:
                variants = m.get('video_info').get('variants')

                video_with_highest_bitrate = sorted(
                    filter(
                        lambda variant: variant.get('bitrate') is not None,
                        variants),
                    key=lambda v: v.get('bitrate'),
                    reverse=True)[0]

                media.append(Video(
                    video_with_highest_bitrate.get('url'),
                ))

        return media

    @staticmethod
    def get_tweet_media(tweet: Status):
        try:
            tweet_media = tweet.extended_entities.get('media')
            if tweet_media is None:
                if tweet.retweeted:
                    tweet_media = tweet.retweeted_status.extended_entities.get('media')
                else:
                    tweet_media = tweet.quoted_status.extended_entities.get('media')
                # If no media is still found then return an empty list
                if tweet_media is None:
                    tweet_media = []

            return tweet_media
        except AttributeError:
            return []
